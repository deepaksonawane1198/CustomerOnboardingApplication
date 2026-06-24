using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Services;

public class FraudService : IFraudService
{
    public Task<CheckResult> EvaluateAsync(OnboardingRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CheckResult.Success("Fraud"));
    }
}