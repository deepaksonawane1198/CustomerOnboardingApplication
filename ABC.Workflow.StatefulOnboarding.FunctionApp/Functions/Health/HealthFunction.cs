using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using ABC.Workflow.StatefulOnboarding.FunctionApp.OpenApi;
using ParameterLocation = Microsoft.OpenApi.Models.ParameterLocation;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.Functions.Health;

public class HealthFunction
{
    [Function(nameof(HealthFunction))]
    [OpenApiOperation(operationId: "HealthCheck", tags: new[] { "Health" })]
    [OpenApiResponseWithBody(
        statusCode: HttpStatusCode.OK,
        contentType: "text/plain",
        bodyType: typeof(string),
        Description = "Health check response")]
    public HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteString("Healthy");
        return response;
    }
}