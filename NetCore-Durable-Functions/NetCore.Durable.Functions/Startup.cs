using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Durable.Functions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NetCore.Durable.Functions
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			var azureServiceTokenProvider = new AzureServiceTokenProvider();

			var keyVaultClient =
				new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

			var _configuration = builder.Services.BuildServiceProvider().GetRequiredService(typeof(IConfiguration)) as IConfiguration;
			
			builder.Services.AddOptions<StorageOptions>()
				.Configure<IConfiguration>((options, configuration) =>
				{
					configuration.GetSection("AzureStorage").Bind(options);
					//options.StorageConnectionString = keyVaultClient.GetSecretAsync(configuration["AzureMsiOptions:KeyVaultName"],
					//	configuration["AzureMsiOptions:StorageConnectionString"]).Result.Value;
				});

            builder.Services.AddOptions<CosmosDbOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    configuration.GetSection("AzureCosmos").Bind(options);
                });

			//builder.Services.AddOptions<MessagingOptions>()
			//	.Configure<IConfiguration>((options, configuration) =>
			//	{
			//		configuration.GetSection("AzureServiceBus").Bind(options);
			//		options.ServiceBusConnectionStringSecondary = string.Empty;
			//		options.ServiceBusConnectionString = keyVaultClient.GetSecretAsync(configuration["AzureMsiOptions:KeyVaultName"],
			//			configuration["AzureMsiOptions:ServiceBusConnectionString"]).Result.Value;
			//	});

			//builder.Services.AddSingleton<IProceduralSystemMessaging, ProceduralSystemMessaging>();

			// CHANGE TO ACCESS TOKEN
			//var dbConnString = keyVaultClient.GetSecretAsync(_configuration?["AzureMsiOptions:KeyVaultName"],
			//	_configuration?["AzureMsiOptions:SqlConnectionString"]).Result.Value;

			//builder.Services.AddDbContext<TranslationsContext>(options =>
			//{
			//	options.UseSqlServer(dbConnString);
			//});
		}
	}
}
