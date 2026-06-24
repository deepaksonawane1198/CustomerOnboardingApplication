using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Orchestrators;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;

public class CompensationActivity
{
    private readonly IProvisioningService _provisioningService;

    public CompensationActivity(IProvisioningService provisioningService)
    {
        _provisioningService = provisioningService;
    }

    [Function(nameof(CompensationActivity))]
    public Task Run([ActivityTrigger] CompensationRequest request)
        => _provisioningService.CompensateAsync(request.Request, request.Reason);
}
