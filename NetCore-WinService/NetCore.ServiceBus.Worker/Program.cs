using NetCore.ServiceBus.Worker;

IHost host = Host.CreateDefaultBuilder(args)
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         config.AddEnvironmentVariables();
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
