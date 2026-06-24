namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;

public class ApprovalDecision
{
    public bool Approved { get; set; }
    public string ReviewedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime ReviewedAtUtc { get; set; } = DateTime.UtcNow;
}