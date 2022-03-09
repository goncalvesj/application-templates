using EventSourcing.Functions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace EventSourcing.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var _configuration = builder.Services.BuildServiceProvider().GetRequiredService(typeof(IConfiguration)) as IConfiguration;
            var _storageConnString = _configuration["StorageQueue"];
            var _queueName = _configuration["QueueName"];

            builder.Services.AddSingleton<IConferenceService>((s) =>
            {
                return new ConferenceService(_storageConnString, _queueName);
            });

            builder.Services.AddSingleton<IProjectionService>((s) =>
            {
                return new ProjectionService(_storageConnString);
            });
        }
    }
}
