using System;
using Akka.Actor;
using Akka.Configuration;
using Hive.Cluster.ServiceDiscovery.Actors;

namespace Hive.Cluster.ServiceDiscovery
{
    public class Startup : MicroserviceStartup
    {
        private IActorRef servicesActor;
        public override Func<Config, string> ActorSystemName { get; } = new Func<Config, string>((cfg) => cfg.GetString("akka.actorsystem"));

        public override void StartActors(ActorSystem system)
        {
            servicesActor = system.ActorOf<ServicesActor>(ServicesActor.ActorName);
        }
    }
}
