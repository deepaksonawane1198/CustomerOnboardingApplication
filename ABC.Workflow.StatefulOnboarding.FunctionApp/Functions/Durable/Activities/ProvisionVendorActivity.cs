using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;

public class ProvisionVendorActivity
{
    private readonly IProvisioningService _provisioningService;

    public ProvisionVendorActivity(IProvisioningService provisioningService)
    {
        _provisioningService = provisioningService;
    }

    [Function(nameof(ProvisionVendorActivity))]
    public Task<ProvisioningResult> Run([ActivityTrigger] OnboardingRequest request)
        => _provisioningService.ProvisionAsync(request);
}