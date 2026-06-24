using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Helpers;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Services;

public class ComplianceService : IComplianceService
{
    public Task<CheckResult> EvaluateAsync(OnboardingRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.Country.Equals("IN", StringComparison.OrdinalIgnoreCase) &&
            !request.Country.Equals("SG", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(CheckResult.Failure("Compliance", "Country is not enabled for the POC.", ErrorCategory.Business));
        }

        if (request.CompanyName.Contains("blocked", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(CheckResult.Failure("Compliance", "Company is blocked by compliance rules.", ErrorCategory.Business));
        }

        return Task.FromResult(CheckResult.Success("Compliance"));
    }
}