namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Models;

public class WorkflowAuditRecord
{
    public string ApplicationId { get; set; } = string.Empty;
    public string InstanceId { get; set; } = string.Empty;
    public string ApplicantType { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty;
    public string WorkflowStatus { get; set; } = string.Empty;
    public string CurrentStage { get; set; } = string.Empty;
    public string FinalReason { get; set; } = string.Empty;
    public string? DownstreamRecordId { get; set; }
    public string RequestPayloadJson { get; set; } = string.Empty;
    public DateTime SubmittedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime LastUpdatedAtUtc { get; set; }
}