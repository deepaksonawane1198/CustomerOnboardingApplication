namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;

public class ProvisioningResult
{
    public bool IsSuccess { get; set; }
    public bool IsTransientFailure { get; set; }
    public string DownstreamRecordId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}