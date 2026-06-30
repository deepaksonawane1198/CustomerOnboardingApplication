using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Models;

public class DuplicateCheckRequest
{
    public string InstanceId { get; set; } = string.Empty;
    public OnboardingRequest Request { get; set; } = new();
}