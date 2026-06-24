namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;

public class WorkflowExecutionResult
{
    public string ApplicationId { get; set; } = string.Empty;
    public string InstanceId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string FinalReason { get; set; } = string.Empty;
    public string? DownstreamRecordId { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime FinishedAtUtc { get; set; }
    public List<CheckResult> StageResults { get; set; } = new();
}