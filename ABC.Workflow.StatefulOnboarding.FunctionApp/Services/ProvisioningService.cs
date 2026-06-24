using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Services;

public class ProvisioningService : IProvisioningService
{
    public Task<ProvisioningResult> ProvisionAsync(OnboardingRequest request, CancellationToken cancellationToken = default)
    {
        if (request.CompanyName.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new ProvisioningResult
            {
                IsSuccess = false,
                IsTransientFailure = true,
                Reason = "Downstream provisioning timed out in the simulation."
            });
        }

        return Task.FromResult(new ProvisioningResult
        {
            IsSuccess = true,
            DownstreamRecordId = $"VENDOR-{request.ApplicationId}"
        });
    }

    public Task CompensateAsync(OnboardingRequest request, string reason, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}