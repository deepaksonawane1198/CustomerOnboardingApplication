namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Responses;

public class WorkflowStatusResponse
{
    public string InstanceId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? RuntimeStatus { get; set; }
    public object? Input { get; set; }
    public object? Output { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? LastUpdatedAt { get; set; }
}