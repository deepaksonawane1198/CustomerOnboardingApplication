namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;

public class WorkflowSettings
{
    public int AutoApprovalRiskThreshold { get; set; } = 70;
    public int ApprovalTimeoutMinutes { get; set; } = 5;
    public List<string> RequiredVendorDocuments { get; set; } = new();
    public List<string> DuplicateTaxIds { get; set; } = new();
    public string ProvisioningTransientFailureKeyword { get; set; } = "timeout";
    public string SuccessTopicName { get; set; } = "onboarding-events";
    public string ManualReviewEventName { get; set; } = "ManualApprovalReceived";
}