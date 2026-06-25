using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Orchestrators;

public class OnboardingOrchestrator
{
    [Function(nameof(OnboardingOrchestrator))]
    public static async Task<WorkflowExecutionResult> Run(
        [OrchestrationTrigger] TaskOrchestrationContext context,
        OnboardingRequest request)
    {
        var result = new WorkflowExecutionResult
        {
            ApplicationId = request.ApplicationId,
            InstanceId = context.InstanceId,
            Status = "InProgress",
            StartedAtUtc = context.CurrentUtcDateTime
        };
    
        // Step 1 - Validate payload first
        var validationResult = await context.CallActivityAsync<CheckResult>(nameof(ValidatePayloadActivity),request);
        result.StageResults.Add(validationResult);

        if (!validationResult.IsSuccess)
        {
            return await RejectAsync(context, result, validationResult.Reason, "Rejected");
        }

        // Step 2 - Run all 4 checks in parallel
        var checks = new[]
        {
            context.CallActivityAsync<CheckResult>(nameof(ValidateDocumentsActivity), request),
            context.CallActivityAsync<CheckResult>(nameof(DuplicateCheckActivity), request),
            context.CallActivityAsync<CheckResult>(nameof(ComplianceCheckActivity), request),
            context.CallActivityAsync<CheckResult>(nameof(FraudCheckActivity), request)
        };

        // Step 3 - Wait until all 4 complete
        await Task.WhenAll(checks);

        // Step 4 - Add all results into the workflow result
        result.StageResults.AddRange(checks.Select(t => t.Result));

        // Step 5 - If any one failed, reject immediately
        var firstFailure = result.StageResults.FirstOrDefault(r => !r.IsSuccess);
        if (firstFailure is not null)
        {
            return await RejectAsync(context, result, firstFailure.Reason, "Rejected");
        }

        if (request.RiskScore >= 70)
        {
            await context.CallActivityAsync(nameof(UpdateWorkflowAuditActivity), new WorkflowAuditUpdate
            {
                InstanceId = context.InstanceId,
                WorkflowStatus = "InProgress",
                CurrentStage = "ManualApprovalPending",
                FinalReason = null,
                DownstreamRecordId = null,
                LastUpdatedAtUtc = context.CurrentUtcDateTime
            });

            using var timeoutCts = new CancellationTokenSource();
            var approvalTimeoutAt = context.CurrentUtcDateTime.AddMinutes(5);
            var approvalTask = context.WaitForExternalEvent<ApprovalDecision>(WorkflowConstants.ManualApprovalEventName);
            var timeoutTask = context.CreateTimer(approvalTimeoutAt, timeoutCts.Token);
            var winner = await Task.WhenAny(approvalTask, timeoutTask);

            if (winner == timeoutTask)
            {
               return await RejectAsync(context, result, "Manual approval timed out.", "Rejected");
            }
            timeoutCts.Cancel();
            var approval = await approvalTask;
            if (!approval.Approved)
            {
                return await RejectAsync(context, result, $"Manual approval rejected. {approval.Reason}", "Rejected");
            }

            await context.CallActivityAsync(nameof(UpdateWorkflowAuditActivity), new WorkflowAuditUpdate
            {
                InstanceId = context.InstanceId,
                WorkflowStatus = "Completed",
                CurrentStage = "Completed",
                FinalReason = approval.Reason,
                DownstreamRecordId = null,
                LastUpdatedAtUtc = context.CurrentUtcDateTime
            });     
            result.StageResults.Add(CheckResult.Success("ManualApproval"));
        }

        ProvisioningResult provisioning = new()
        {
            IsSuccess = false,
            IsTransientFailure = false,
            Reason = "Provisioning not attempted."
        };
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            provisioning = await context.CallActivityAsync<ProvisioningResult>(
                nameof(ProvisionVendorActivity),
                request);
            if (provisioning.IsSuccess)
            {
                break;
            }  

            if (provisioning.IsTransientFailure && attempt < 3)
            {
                await context.CallActivityAsync(nameof(UpdateWorkflowAuditActivity), new WorkflowAuditUpdate
                {
                    InstanceId = context.InstanceId,
                    WorkflowStatus = "InProgress",
                    CurrentStage = $"ProvisioningRetry-{attempt}",
                    FinalReason = provisioning.Reason,
                    DownstreamRecordId = null,
                    LastUpdatedAtUtc = context.CurrentUtcDateTime
                });
                var retryAt = context.CurrentUtcDateTime.AddSeconds(Math.Pow(2, attempt) * 5);
                await context.CreateTimer(retryAt, CancellationToken.None);
                continue;
            }
            break;
        }
        if (!provisioning.IsSuccess)
        {
            await context.CallActivityAsync(
                nameof(CompensationActivity),
                new CompensationRequest(request, provisioning.Reason));

            return await RejectAsync(context, result, provisioning.Reason, "TechnicalFailure");
        }

        // Step 6 - If all checks passed, mark completed for now
        result.DownstreamRecordId = provisioning.DownstreamRecordId;
        result.Status = "Completed";
        result.FinishedAtUtc = context.CurrentUtcDateTime;
        await context.CallActivityAsync(nameof(UpdateWorkflowAuditActivity), new WorkflowAuditUpdate
            {
                InstanceId = context.InstanceId,
                WorkflowStatus = "Completed",
                CurrentStage = "Completed",
                FinalReason = null,
                DownstreamRecordId = provisioning.DownstreamRecordId,
                LastUpdatedAtUtc = context.CurrentUtcDateTime
            }); 
        return result;
    }
    private static async Task<WorkflowExecutionResult> RejectAsync(TaskOrchestrationContext context,WorkflowExecutionResult result,string reason,
    string finalStatus)
    {
        result.Status = finalStatus;
        result.FinalReason = reason;
        result.FinishedAtUtc = context.CurrentUtcDateTime;
        await context.CallActivityAsync(nameof(UpdateWorkflowAuditActivity), new WorkflowAuditUpdate
        {
            InstanceId = context.InstanceId,
            WorkflowStatus = finalStatus,
            CurrentStage = finalStatus,
            FinalReason = reason,
            DownstreamRecordId = null,
            LastUpdatedAtUtc = context.CurrentUtcDateTime
        });
        return result;
    }
}
public record CompensationRequest(OnboardingRequest Request, string Reason);