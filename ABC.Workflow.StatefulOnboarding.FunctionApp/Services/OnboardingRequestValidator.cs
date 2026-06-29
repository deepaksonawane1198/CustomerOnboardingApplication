using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Services;

public static class OnboardingRequestValidator
{
    public static List<string> Validate(OnboardingRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.ApplicationId)) errors.Add("applicationId is required.");
        if (string.IsNullOrWhiteSpace(request.ApplicantType)) errors.Add("applicantType is required.");
        if (string.IsNullOrWhiteSpace(request.CompanyName)) errors.Add("companyName is required.");
        if (string.IsNullOrWhiteSpace(request.TaxId)) errors.Add("taxId is required.");
        if (string.IsNullOrWhiteSpace(request.Country)) errors.Add("country is required.");
        if (string.IsNullOrWhiteSpace(request.ContactEmail)) errors.Add("contactEmail is required.");
        if (request.SubmittedAt == default) errors.Add("submittedAt is required.");

        return errors;
    }
}