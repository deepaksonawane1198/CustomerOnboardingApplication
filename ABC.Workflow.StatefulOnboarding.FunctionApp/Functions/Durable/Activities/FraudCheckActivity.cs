using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;

public class FraudCheckActivity
{
    private readonly IFraudService _fraudService;

    public FraudCheckActivity(IFraudService fraudService)
    {
        _fraudService = fraudService;
    }

    [Function(nameof(FraudCheckActivity))]
    public Task<CheckResult> Run([ActivityTrigger] OnboardingRequest request)
        => _fraudService.EvaluateAsync(request);
}