using System;
using System.Threading;
using Azure.Identity;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NetCore.WinService
{
    public static class Program
    {
        private static IConfigurationRefresher _refresher;
        private static Timer _timer;

        private static readonly AzureServiceTokenProvider AzureServiceTokenProvider = new AzureServiceTokenProvider();

        private static readonly KeyVaultClient KeyVaultClient =
            new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(AzureServiceTokenProvider.KeyVaultTokenCallback));

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                // Configures app as Windows Service
                .UseWindowsService()
                // Configures other services, Options objects
                .ConfigureServices((hostContext, services) =>
                {
                    var storageSettings = hostContext.Configuration.GetSection("Storage");
                    services.Configure<StorageSettings>(storageSettings);

                    services.AddHostedService<Worker>();
                    services.AddApplicationInsightsTelemetryWorkerService();
                })
                // Sets up Azure App Config
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var settings = config.Build();

                    var applicationConfigurationEndpoint = hostContext.HostingEnvironment.IsDevelopment()
                        // Gets Azure App Config details from KeyVault
                        ? KeyVaultClient
                            .GetSecretAsync(settings["KeyVault"], "ApplicationConfiguration-ConnectionString")
                            .Result.Value
                        // Falls back to app.settings.json
                        : settings["AppConfiguration:ConnectionString"];

                    config.AddAzureAppConfiguration(options =>
                    {
                        var appConfigurationOptions = hostContext.HostingEnvironment.IsDevelopment()
                            // Uses connections string
                            ? options.Connect(applicationConfigurationEndpoint)
                            // Uses Azure Managed Identity
                            : options.Connect(new Uri(applicationConfigurationEndpoint),
                                new ManagedIdentityCredential());

                        appConfigurationOptions
                            .ConfigureRefresh(refresh =>
                            {
                                refresh.Register("Storage:ConnectionString", "Label1")
                                    .Register("ApplicationInsights:InstrumentationKey", "Label1")
                                    .SetCacheExpiration(TimeSpan.FromSeconds(1));
                            })
                            .UseAzureKeyVault(KeyVaultClient);

                        // Settings Auto-Refresh
                        _refresher = options.GetRefresher();

                        _timer = new Timer(async _ => await _refresher.Refresh().ConfigureAwait(false),
                            null,
                            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
                    });
                });
        }
    }
}