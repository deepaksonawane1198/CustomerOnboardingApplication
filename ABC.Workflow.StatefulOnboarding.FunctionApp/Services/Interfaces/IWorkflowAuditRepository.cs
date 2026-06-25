using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;

public interface IWorkflowAuditRepository
{
    Task CreateAsync(WorkflowAuditRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkflowAuditUpdate update, CancellationToken cancellationToken = default);
}