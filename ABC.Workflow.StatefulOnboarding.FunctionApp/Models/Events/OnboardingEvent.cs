namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Events;

public class OnboardingEvent
{
    public string EventType { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? DownstreamRecordId { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}