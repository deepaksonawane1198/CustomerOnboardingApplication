using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;

public class OnboardingRequest
{
    public string ApplicationId { get; set; } = string.Empty;
    public string ApplicantType { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public List<DocumentMetadata> Documents { get; set; } = new();
    public string SourceSystem { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}