namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Responses;

public class WorkflowStartResponse
{
    public string InstanceId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string StatusQueryUri { get; set; } = string.Empty;
    public string ApprovalCallbackUri { get; set; } = string.Empty;
}