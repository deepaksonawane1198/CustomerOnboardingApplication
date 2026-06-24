using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;

public interface IProvisioningService
{
    Task<ProvisioningResult> ProvisionAsync(OnboardingRequest request, CancellationToken cancellationToken = default);
    Task CompensateAsync(OnboardingRequest request, string reason, CancellationToken cancellationToken = default);
}