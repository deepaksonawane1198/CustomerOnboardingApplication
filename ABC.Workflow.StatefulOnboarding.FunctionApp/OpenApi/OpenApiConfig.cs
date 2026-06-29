using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace ABC.Workflow.StatefulOnboarding.FunctionApp.OpenApi;

public class OpenApiConfig : DefaultOpenApiConfigurationOptions
{
    public override OpenApiInfo Info { get; set; } = new OpenApiInfo
    {
        Title = "Customer Onboarding API",
        Version = "1.0.0",
        Description = "Stateful onboarding workflow using Azure Durable Functions and APIM.",
        Contact = new OpenApiContact
        {
            Name = "Deepak Sonawane"
        }
    };

    public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
}