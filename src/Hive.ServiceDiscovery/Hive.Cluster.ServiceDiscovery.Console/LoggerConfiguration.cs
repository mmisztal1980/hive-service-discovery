using Serilog;

namespace Hive.Cluster.ServiceDiscovery
{
    public static class LoggerConfiguration
    {
        public static ILogger ConfigureLogger()
        {
            var logger = new Serilog.LoggerConfiguration()
                .ReadFrom.AppSettings()
                .Enrich.WithProperty("ServiceType", "Hive.Cluster.ServiceDiscovery")
                .CreateLogger();

            return logger;
        }
    }
}
