using System;
using System.Threading;

namespace Hive.Cluster.ServiceDiscovery
{
    class Program
    {
        private static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Serilog.Log.Logger = LoggerConfiguration.ConfigureLogger();

            AppDomain.CurrentDomain.ProcessExit += ProcessExit;

            Microservice.StartAsync<Startup>(cancellationTokenSource.Token).Wait();

            Serilog.Log.Logger.Information("Process Exited");
        }

        private static void ProcessExit(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
