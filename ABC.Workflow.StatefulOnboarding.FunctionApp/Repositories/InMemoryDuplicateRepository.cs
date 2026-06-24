using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories;

public class InMemoryDuplicateRepository : IDuplicateRepository
{
    private readonly HashSet<string> _duplicateTaxIds;

    public InMemoryDuplicateRepository(IOptions<WorkflowSettings> options)
    {
        _duplicateTaxIds = options.Value.DuplicateTaxIds
            .Select(x => x.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public Task<bool> ExistsAsync(string taxId, CancellationToken cancellationToken = default)
        => Task.FromResult(_duplicateTaxIds.Contains(taxId));
}