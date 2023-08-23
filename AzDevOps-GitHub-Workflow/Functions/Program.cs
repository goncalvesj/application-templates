using AzDO.GH.Function;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddTransient<IGitHubClientService, GitHubClientService>();
        services.AddTransient<IAzDOClientService, AzDOClientService>();
    })
    .Build();

host.Run();
