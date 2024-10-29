using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCore.WinService;
using System;
using System.Threading;
using System.Threading.Tasks;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly TelemetryClient _telemetryClient;
    private StorageSettings _storageSettings;
    private readonly IConfiguration _configuration;
    private readonly IConfigurationRefresher _refresher;

    public Worker(ILogger<Worker> logger, IOptionsMonitor<StorageSettings> storageSettings, TelemetryClient tc, IConfiguration configuration, IConfigurationRefresher refresher)
    {
        _logger = logger;
        _telemetryClient = tc;

        _telemetryClient.TrackAvailability(new AvailabilityTelemetry
        {
            Success = true,
            Name = "Running"
        });

        // Settings Auto-Refresh
        storageSettings.OnChange(settings => _storageSettings = settings);
        _storageSettings = storageSettings.CurrentValue;

        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _refresher = refresher ?? throw new ArgumentNullException(nameof(refresher));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (_telemetryClient.StartOperation<RequestTelemetry>("operation"))
            {
                try
                {
                    _ = _refresher.TryRefreshAsync();
                    await DoWork(stoppingToken);
                }
                catch (Exception e)
                {
                    LogException(e, $"Error Message - {e.Message}");
                }
                await Task.Delay(30000, stoppingToken);
            }
        }
    }

    private async Task<bool> DoWork(CancellationToken stoppingToken)
    {
        LogEvent("DoWork!");

        Console.WriteLine(_configuration["Storage:ConnectionString"] ?? "No data.");
        Console.WriteLine(_storageSettings.ConnectionString);

        return await Task.FromResult(true);
    }


    private void LogEvent(string text)
    {
        _logger.LogInformation(text);
        _telemetryClient.TrackEvent(text);
    }

    private void LogException(Exception ex, string text)
    {
        _logger.LogError(ex, text);
        _telemetryClient.TrackException(ex);
    }
}