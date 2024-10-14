using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TcpListener _listener;
        private const int port = 5678;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _listener = new TcpListener(IPAddress.Any, port);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _listener.Start();

            _logger.LogInformation($"Server started. Listening to TCP clients at {IPAddress.Any}:{port}");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting for connection...");
                var client = _listener.AcceptTcpClient();

                _logger.LogInformation("Connection accepted.");
                var ns = client.GetStream();

                var byteTime = Encoding.ASCII.GetBytes(DateTime.Now.ToString());

                try
                {
                    ns.Write(byteTime, 0, byteTime.Length);
                    ns.Close();
                    client.Close();
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e.ToString());
                }

                await Task.Delay(1000, stoppingToken);
            }

            _listener.Stop();
        }
    }
}