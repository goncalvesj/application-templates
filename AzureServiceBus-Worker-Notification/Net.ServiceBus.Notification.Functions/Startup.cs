using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Net.ServiceBus.Notification.Functions.Startup))]
namespace Net.ServiceBus.Notification.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {          
        }
    }
}
