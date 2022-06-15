using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignalR.Functions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SignalR.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var _configuration = builder.Services.BuildServiceProvider().GetRequiredService(typeof(IConfiguration)) as IConfiguration;

            builder.Services.AddOptions<ServiceBusOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    options.ConnectionString = configuration["ServiceBusConnection"];
                });
        }
    }
    public class ServiceBusOptions
    {
        public string ConnectionString { get; set; }
    }
}
