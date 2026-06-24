using ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Helpers;
namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;

public class CheckResult
{
    public bool IsSuccess { get; set; }
    public string Stage { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public ErrorCategory ErrorCategory { get; set; } = ErrorCategory.None;

    public static CheckResult Success(string stage) => new() { IsSuccess = true, Stage = stage };
    public static CheckResult Failure(string stage, string reason, ErrorCategory category) => new()
    {
        IsSuccess = false,
        Stage = stage,
        Reason = reason,
        ErrorCategory = category
    };
}