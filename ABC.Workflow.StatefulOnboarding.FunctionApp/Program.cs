using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Common.Connectors;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories.CustomerMasterSystem.Interfaces;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker =>{worker.UseNewtonsoftJson();}).ConfigureOpenApi()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<WorkflowSettings>(context.Configuration.GetSection("WorkflowSettings"));
        services.AddSingleton<IComplianceService, ComplianceService>();
        services.AddSingleton<IFraudService, FraudService>();
        services.AddSingleton<IProvisioningService, ProvisioningService>();
        services.AddSingleton<IWorkflowAuditRepository, CustomerMasterSystemWorkflowAuditRepository>();
        services.AddSingleton<SqlConnectionFactory>();
    })
    .Build();

host.Run();