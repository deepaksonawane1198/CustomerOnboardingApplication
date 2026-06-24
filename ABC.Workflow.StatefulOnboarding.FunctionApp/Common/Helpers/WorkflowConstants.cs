namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Helpers;
public static class WorkflowConstants
{
    public const string ManualApprovalEventName = "ManualApprovalReceived";
    public const string ApprovedEventType = "OnboardingApproved";
    public const string RejectedEventType = "OnboardingRejected";
    public const string FailedEventType = "OnboardingProvisioningFailed";
}