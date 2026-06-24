using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;

public class ComplianceCheckActivity
{
    private readonly IComplianceService _complianceService;

    public ComplianceCheckActivity(IComplianceService complianceService)
    {
        _complianceService = complianceService;
    }

    [Function(nameof(ComplianceCheckActivity))]
    public Task<CheckResult> Run([ActivityTrigger] OnboardingRequest request)
        => _complianceService.EvaluateAsync(request);
}