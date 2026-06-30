using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Models;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Interfaces;

public interface IWorkflowAuditRepository
{
    Task CreateAsync(WorkflowAuditRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkflowAuditUpdate update, CancellationToken cancellationToken = default);
    Task<bool> IsDuplicateAsync(string taxId,string currentInstanceId,CancellationToken cancellationToken = default);
}