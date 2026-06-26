using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Models;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Interfaces;
using Microsoft.Azure.Functions.Worker;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;

public class UpdateWorkflowAuditActivity
{
    private readonly IWorkflowAuditRepository _workflowAuditRepository;

    public UpdateWorkflowAuditActivity(IWorkflowAuditRepository workflowAuditRepository)
    {
        _workflowAuditRepository = workflowAuditRepository;
    }

    [Function(nameof(UpdateWorkflowAuditActivity))]
    public Task Run([ActivityTrigger] WorkflowAuditUpdate update)
        => _workflowAuditRepository.UpdateAsync(update);
}