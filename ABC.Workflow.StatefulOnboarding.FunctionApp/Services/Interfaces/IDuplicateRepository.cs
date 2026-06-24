namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;

public interface IDuplicateRepository
{
    Task<bool> ExistsAsync(string taxId, CancellationToken cancellationToken = default);
}