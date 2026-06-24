using ABC.Workflow.StatefulOnboarding.FunctionApp.Services;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Repositories;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Services.Interfaces;
using ABC.Workflow.StatefulOnboarding.FunctionApp.Models.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<WorkflowSettings>(context.Configuration.GetSection("WorkflowSettings"));
        services.AddSingleton<IDuplicateRepository, InMemoryDuplicateRepository>();
        services.AddSingleton<IComplianceService, ComplianceService>();
        services.AddSingleton<IFraudService, FraudService>();
        services.AddSingleton<IProvisioningService, ProvisioningService>();
    })
    .Build();

host.Run();