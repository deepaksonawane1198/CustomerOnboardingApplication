using ABC.Workflow.StatefulOnboarding.FunctionApp.Services;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Helpers;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using Microsoft.Azure.Functions.Worker;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Activities;

public class ValidatePayloadActivity
{
    [Function(nameof(ValidatePayloadActivity))]
    public CheckResult Run([ActivityTrigger] OnboardingRequest request)
    {
        var errors = OnboardingRequestValidator.Validate(request);

        return errors.Count == 0
            ? CheckResult.Success("ValidatePayload")
            : CheckResult.Failure("ValidatePayload", string.Join(" ", errors), ErrorCategory.Business);
    }
}