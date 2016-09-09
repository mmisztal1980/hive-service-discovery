using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util;

namespace Hive.Cluster
{
    public class Microservice
    {
        public static AtomicBoolean IsClustered;
        private static ActorSystem actorSystem;
        private static Akka.Cluster.Cluster cluster;

        private static readonly ManualResetEventSlim ClusterLeaveEvent = new ManualResetEventSlim(false);

        public static async Task StartAsync<TStartup>(CancellationToken cancellationToken)
            where TStartup : MicroserviceStartup, new()
        {
            actorSystem = StartInternal<TStartup>();
            cluster = Akka.Cluster.Cluster.Get(actorSystem);

            cluster.RegisterOnMemberUp(() =>
            {
                IsClustered = true;
            });

            cluster.RegisterOnMemberRemoved(() =>
            {
                if (IsClustered)
                {
                    IsClustered = false;
                    ClusterLeaveEvent.Set();
                }
            });

            await Task.Delay(TimeSpan.FromMilliseconds(-1), cancellationToken);

            if (IsClustered)
            {
                cluster.Leave(cluster.SelfAddress);

                if (!ClusterLeaveEvent.WaitHandle.WaitOne(10.Seconds()))
                {
                    cluster.Down(cluster.SelfAddress);
                }
            }

            await actorSystem.Terminate();
        }

        private static ActorSystem StartInternal<TStartup>()
            where TStartup : MicroserviceStartup, new()
        {
            var startup = new TStartup();

            startup.Configure();

            var config = startup.ConfigureActorSystem();

            var system = ActorSystem.Create(startup.ActorSystemName(config), config);

            startup.StartActors(system);

            return system;
        }
    }
}
