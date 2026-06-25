namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;

public class WorkflowAuditUpdate
{
    public string InstanceId { get; set; } = string.Empty;
    public string WorkflowStatus { get; set; } = string.Empty;
    public string CurrentStage { get; set; } = string.Empty;
    public string? FinalReason { get; set; }
    public string? DownstreamRecordId { get; set; }
    public DateTime LastUpdatedAtUtc { get; set; }
}