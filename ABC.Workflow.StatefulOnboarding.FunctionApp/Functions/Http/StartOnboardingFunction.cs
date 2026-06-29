using System.Net;
using System.Text.Json;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Requests;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Responses;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Durable.Orchestrators;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Models;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Interfaces;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;



namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Http;

public class StartOnboardingFunction
{
    private readonly ILogger<StartOnboardingFunction> _logger;
    private readonly IWorkflowAuditRepository _workflowAuditRepository;

    public StartOnboardingFunction(ILogger<StartOnboardingFunction> logger, IWorkflowAuditRepository workflowAuditRepository)
    {
        _logger = logger;
        _workflowAuditRepository = workflowAuditRepository;
    }

    [Function(nameof(StartOnboardingFunction))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "workflow/start")] HttpRequestData req,
        [DurableClient] DurableTaskClient durableClient)
    {
        var request = await JsonSerializer.DeserializeAsync<OnboardingRequest>(
            req.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (request is null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request body.");
            return badResponse;
        }

        var instanceId = await durableClient.ScheduleNewOrchestrationInstanceAsync(
            nameof(OnboardingOrchestrator),
            request);

        await _workflowAuditRepository.CreateAsync(new WorkflowAuditRecord
        {
            ApplicationId = request.ApplicationId,
            InstanceId = instanceId,
            ApplicantType = request.ApplicantType,
            CompanyName = request.CompanyName,
            TaxId = request.TaxId,
            Country = request.Country,
            ContactEmail = request.ContactEmail,
            SourceSystem = request.SourceSystem,
            WorkflowStatus = "InProgress",
            CurrentStage = "Started",
            FinalReason = string.Empty,
            DownstreamRecordId = null,
            RequestPayloadJson = JsonSerializer.Serialize(request),
            SubmittedAtUtc = request.SubmittedAt,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedAtUtc = DateTime.UtcNow
        });

        _logger.LogInformation("Started onboarding orchestration. InstanceId={InstanceId}, ApplicationId={ApplicationId}",
            instanceId, request.ApplicationId);

        var response = req.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteAsJsonAsync(new WorkflowStartResponse
        {
            InstanceId = instanceId,
            ApplicationId = request.ApplicationId,
            StatusQueryUri = $"{req.Url.Scheme}://{req.Url.Authority}/api/workflow/status/{instanceId}",
            ApprovalCallbackUri = $"{req.Url.Scheme}://{req.Url.Authority}/api/workflow/approve/{instanceId}"
        });

        return response;
    }
}