using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;

public class ValidateDocumentsActivity
{
    private readonly WorkflowSettings _settings;

    public ValidateDocumentsActivity(IOptions<WorkflowSettings> options)
    {
        _settings = options.Value;
    }

    [Function(nameof(ValidateDocumentsActivity))]
    public CheckResult Run([ActivityTrigger] OnboardingRequest request)
    {
        var documentTypes = request.Documents.Select(d => d.Type).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = _settings.RequiredVendorDocuments.Where(required => !documentTypes.Contains(required)).ToList();

        return missing.Count == 0
            ? CheckResult.Success("ValidateDocuments")
            : CheckResult.Failure("ValidateDocuments", $"Missing required documents: {string.Join(", ", missing)}", ErrorCategory.Business);
    }
}