using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services => {
        services.AddTransient(s => new GitHubClient(new ProductHeaderValue(Environment.GetEnvironmentVariable("PRODUCT_HEADER_VALUE")))
        {
            Credentials = new Credentials(Environment.GetEnvironmentVariable("GITHUB_PAT"))
        });
    })
    .Build();

host.Run();
