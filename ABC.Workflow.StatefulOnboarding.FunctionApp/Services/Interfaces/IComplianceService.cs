using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;

public interface IComplianceService
{
    Task<CheckResult> EvaluateAsync(OnboardingRequest request, CancellationToken cancellationToken = default);
}