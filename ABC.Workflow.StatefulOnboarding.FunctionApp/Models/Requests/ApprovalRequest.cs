namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;

public class ApprovalRequest
{
    public bool Approved { get; set; }
    public string ReviewedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}