using System.Net;
using System.Text.Json;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Helpers;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using ABC.Workflow.StatefulOnboarding.FunctionApp.OpenApi;
using ParameterLocation = Microsoft.OpenApi.Models.ParameterLocation;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Http;

public class ApprovalCallbackFunction
{
    private readonly ILogger<ApprovalCallbackFunction> _logger;

    public ApprovalCallbackFunction(ILogger<ApprovalCallbackFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ApprovalCallbackFunction))]
    [OpenApiOperation(
        operationId: "ApproveWorkflow",
        tags: new[] { "Workflow" },
        Summary = "Approve or reject workflow",
        Description = "Raises manual approval event for a running durable workflow instance.")]
    [OpenApiParameter(
        name: "instanceId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "Durable workflow instance id")]
    [OpenApiRequestBody(
        contentType: "application/json",
        bodyType: typeof(ApprovalRequest),
        Required = true,
        Description = "Approval decision payload")]
    [OpenApiResponseWithoutBody(
        statusCode: HttpStatusCode.Accepted,
        Description = "Approval event accepted")]

    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "workflow/approve/{instanceId}")] HttpRequestData req,
        string instanceId,
        [DurableClient] DurableTaskClient durableClient)
    {
        var request = await JsonSerializer.DeserializeAsync<ApprovalRequest>(
            req.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (request is null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Approval request was empty.");
            return badResponse;
        }

        var decision = new ApprovalDecision
        {
            Approved = request.Approved,
            ReviewedBy = request.ReviewedBy,
            Reason = request.Reason,
            ReviewedAtUtc = DateTime.UtcNow
        };

        await durableClient.RaiseEventAsync(
            instanceId,
            WorkflowConstants.ManualApprovalEventName,
            decision);

        _logger.LogInformation("Raised manual approval event for InstanceId={InstanceId}", instanceId);

        var response = req.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteStringAsync("Approval event accepted.");
        return response;
    }
}