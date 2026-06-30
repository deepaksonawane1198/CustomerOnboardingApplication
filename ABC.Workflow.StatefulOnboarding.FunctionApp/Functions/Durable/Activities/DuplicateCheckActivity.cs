using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Models;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Interfaces;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Helpers;
using Microsoft.Azure.Functions.Worker;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;

public class DuplicateCheckActivity
{
    private readonly IWorkflowAuditRepository _repository;

    public DuplicateCheckActivity(IWorkflowAuditRepository repository)
    {
        _repository = repository;
    }

    [Function(nameof(DuplicateCheckActivity))]
    public async Task<CheckResult> Run([ActivityTrigger] DuplicateCheckRequest input)
    {
        var isDuplicate = await _repository.IsDuplicateAsync(
            input.Request.TaxId,
            input.InstanceId);

        return isDuplicate
            ? CheckResult.Failure(
                "DuplicateCheck",
                $"Duplicate TaxId '{input.Request.TaxId}' found.",
                ErrorCategory.Business)
            : CheckResult.Success("DuplicateCheck");
    }
}