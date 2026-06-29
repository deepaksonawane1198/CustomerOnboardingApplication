using System.Net;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Http;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Responses;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

public class GetWorkflowStatusFunction
{
    [Function(nameof(GetWorkflowStatusFunction))]
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

