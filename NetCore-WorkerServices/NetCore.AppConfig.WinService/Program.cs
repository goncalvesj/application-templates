using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCore.WinService;
using System;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ".NET Woker Service";
});

var storageSettings = builder.Configuration.GetSection("Storage");
builder.Services.Configure<StorageSettings>(storageSettings);

builder.Services.AddHostedService<Worker>();
builder.Services.AddApplicationInsightsTelemetryWorkerService();

builder.Configuration.AddAzureAppConfiguration(options =>
{
    var appConfigurationOptions = options.Connect(Environment.GetEnvironmentVariable("AppConfiguration:ConnectionString"))
        .ConfigureKeyVault(keyVaultOptions =>
        {
            keyVaultOptions.SetCredential(new DefaultAzureCredential());
        });

    appConfigurationOptions
        .ConfigureRefresh(refresh =>
        {
            refresh.Register("Storage:ConnectionString", "Label1")
                .Register("ApplicationInsights:InstrumentationKey", "Label1")
                .SetCacheExpiration(TimeSpan.FromSeconds(60));
        });

    builder.Services.AddSingleton(options.GetRefresher());
});

var host = builder.Build();
host.Run();