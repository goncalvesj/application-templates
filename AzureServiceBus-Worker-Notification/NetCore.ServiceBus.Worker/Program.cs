using NetCore.ServiceBus.Worker;

IHost host = Host.CreateDefaultBuilder(args)
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         var secretsPath = Environment.GetEnvironmentVariable("CONFIG_FILES_PATH");
         
         config.AddEnvironmentVariables();
         config.AddKeyPerFile(directoryPath: $"{secretsPath}", optional: true);
     })
      .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
      {
          loggingBuilder.AddConsole();
      })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddApplicationInsightsTelemetryWorkerService();
    })
    .Build();

await host.RunAsync();
