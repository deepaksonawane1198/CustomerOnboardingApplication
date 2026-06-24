using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Helpers;
using Microsoft.Azure.Functions.Worker;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;

public class DuplicateCheckActivity
{
    private readonly IDuplicateRepository _repository;

    public DuplicateCheckActivity(IDuplicateRepository repository)
    {
        _repository = repository;
    }

    [Function(nameof(DuplicateCheckActivity))]
    public async Task<CheckResult> Run([ActivityTrigger] OnboardingRequest request)
    {
        var exists = await _repository.ExistsAsync(request.TaxId);

        return exists
            ? CheckResult.Failure("DuplicateCheck", "Duplicate taxId found.", ErrorCategory.Business)
            : CheckResult.Success("DuplicateCheck");
    }
}