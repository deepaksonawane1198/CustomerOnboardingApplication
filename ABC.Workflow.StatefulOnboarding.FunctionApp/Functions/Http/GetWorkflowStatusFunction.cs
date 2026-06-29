using System.Net;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Responses;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using ParameterLocation = Microsoft.OpenApi.Models.ParameterLocation;

public class GetWorkflowStatusFunction
{
    [Function(nameof(GetWorkflowStatusFunction))]
    [OpenApiOperation(
        operationId: "GetWorkflowStatus",
        tags: new[] { "Workflow" },
        Summary = "Get workflow status",
        Description = "Returns durable workflow runtime status and output.")]
    [OpenApiParameter(
        name: "instanceId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "Durable workflow instance id")]
    [OpenApiResponseWithBody(
        statusCode: HttpStatusCode.OK,
        contentType: "application/json",
        bodyType: typeof(WorkflowStatusResponse),
        Description = "Workflow status response")]
    [OpenApiResponseWithoutBody(
        statusCode: HttpStatusCode.NotFound,
        Description = "Workflow instance not found")]

public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "workflow/status/{instanceId}")] HttpRequestData req,
        string instanceId,
        [DurableClient] DurableTaskClient durableClient)
    {
        var metadata = await durableClient.GetInstanceAsync(instanceId, getInputsAndOutputs: true);

        if (metadata is null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync($"No orchestration instance found for {instanceId}.");
            return notFound;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new WorkflowStatusResponse
        {
            InstanceId = instanceId,
            Name = metadata.Name,
            RuntimeStatus = metadata.RuntimeStatus.ToString(),
            Input = metadata.SerializedInput,
            Output = metadata.SerializedOutput,
            CreatedAt = metadata.CreatedAt,
            LastUpdatedAt = metadata.LastUpdatedAt
        });

        return response;
    }
}

