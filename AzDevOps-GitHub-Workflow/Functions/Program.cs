using AzDO.GH.Function;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddTransient<IGitHubClientService, GitHubClientService>();
        services.AddTransient<IAzDOClientService, AzDOClientService>();
    })
    .Build();

host.Run();
