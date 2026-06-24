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

        var validationResult = await context.CallActivityAsync<CheckResult>(nameof(ValidatePayloadActivity), request);
        result.StageResults.Add(validationResult);
        if (!validationResult.IsSuccess)
        {
            result.Status = "Rejected";
            result.FinalReason = validationResult.Reason;
            result.FinishedAtUtc = context.CurrentUtcDateTime;
            return result;
        }
        
        var documentResult = await context.CallActivityAsync<CheckResult>(nameof(ValidateDocumentsActivity), request);
        result.StageResults.Add(documentResult);

        if (!documentResult.IsSuccess)
        {
            result.Status = "Rejected";
            result.FinalReason = documentResult.Reason;
            result.FinishedAtUtc = context.CurrentUtcDateTime;
            return result;
        }

        var duplicateResult = await context.CallActivityAsync<CheckResult>(nameof(DuplicateCheckActivity), request);
        result.StageResults.Add(duplicateResult);
        if (!duplicateResult.IsSuccess)
        {
            result.Status = "Rejected";
            result.FinalReason = duplicateResult.Reason;
            result.FinishedAtUtc = context.CurrentUtcDateTime;
            return result;
        }

        var complianceResult = await context.CallActivityAsync<CheckResult>(nameof(ComplianceCheckActivity), request);
        result.StageResults.Add(complianceResult);
        if (!complianceResult.IsSuccess)
        {
            result.Status = "Rejected";
            result.FinalReason = complianceResult.Reason;
            result.FinishedAtUtc = context.CurrentUtcDateTime;
            return result;
        }

        var fraudResult = await context.CallActivityAsync<CheckResult>(nameof(FraudCheckActivity), request);
        result.StageResults.Add(fraudResult);
        if (!fraudResult.IsSuccess)
        {
            result.Status = "Rejected";
            result.FinalReason = fraudResult.Reason;
            result.FinishedAtUtc = context.CurrentUtcDateTime;
            return result;
        }

        if (request.RiskScore >= 70)
        {
            using var timeoutCts = new CancellationTokenSource();
            var approvalTimeoutAt = context.CurrentUtcDateTime.AddMinutes(5);
            var approvalTask = context.WaitForExternalEvent<ApprovalDecision>(WorkflowConstants.ManualApprovalEventName);
            var timeoutTask = context.CreateTimer(approvalTimeoutAt, timeoutCts.Token);
            var winner = await Task.WhenAny(approvalTask, timeoutTask);
            if (winner == timeoutTask)
            {
                result.Status = "Rejected";
                result.FinalReason = "Manual approval timed out.";
                result.FinishedAtUtc = context.CurrentUtcDateTime;
                return result;
            }
            timeoutCts.Cancel();
            var approval = await approvalTask;
            if (!approval.Approved)
            {
                result.Status = "Rejected";
                result.FinalReason = $"Manual approval rejected. {approval.Reason}";
                result.FinishedAtUtc = context.CurrentUtcDateTime;
                return result;
            }
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

            result.Status = "TechnicalFailure";
            result.FinalReason = provisioning.Reason;
            result.FinishedAtUtc = context.CurrentUtcDateTime;
            return result;
        }

        result.DownstreamRecordId = provisioning.DownstreamRecordId;
        result.Status = "Completed";
        result.FinishedAtUtc = context.CurrentUtcDateTime;
        return result;
    }
}
public record CompensationRequest(OnboardingRequest Request, string Reason);