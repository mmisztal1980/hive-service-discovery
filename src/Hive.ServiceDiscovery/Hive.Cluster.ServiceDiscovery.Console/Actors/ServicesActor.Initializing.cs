using System;
using System.Linq;
using Akka.Actor;
using Akka.Cluster;
using Akka.Routing;
using Akka.Util.Internal;
using SvcDiscovery = Hive.Cluster.Messages.ServiceDiscovery;

namespace Hive.Cluster.ServiceDiscovery.Actors
{
    public partial class ServicesActor
    {
        private int ack;
        private bool initialized = false;

        private void Initializing()
        {
            Log.Info("Initializing");
            initialized = false;

            if (IsRoleLeader)
            {
                Log.Info("Become Ready");
                Become(Ready);
                return;
            }

            Receive<ReceiveTimeout>(x => OnReceiveTimeout(x));

            Receive<SvcDiscovery.SetServices>(x => OnServices(x));

            Receive<ClusterEvent.UnreachableMember>(x => OnUnreachableMember(x));

            Receive<SvcDiscovery.GetServices>(x => Log.Debug("Received GetServices while Initializing"));

            Receive<Initialize>(x => OnInitialize(x));

            Self.Tell(new Initialize());
        }

        private void OnInitialize(Initialize msg)
        {
            Log.Info("{@Event}", msg);

            ack = ClusterBroadcast.Ask<Routees>(new GetRoutees()).Result.Members.Count();

            Context.SetReceiveTimeout(TimeSpan.FromSeconds(3));

            ClusterBroadcast.Tell(new SvcDiscovery.GetServices());
        }

        private void OnServices(SvcDiscovery.SetServices msg)
        {
            Log.Info("{@Event}", msg);

            initialized = true;

            msg.Services.ForEach(x =>
            {
                if (!Services.Any(svc => svc.ClusterAddress.Equals(x.ClusterAddress)))
                    Services.Add(x);
            });

            DecrementAck();
        }

        private void OnReceiveTimeout(ReceiveTimeout msg)
        {
            Log.Info("{@Event}", msg);

            DecrementAck();
        }

        private void DecrementAck()
        {
            ack--;

            Log.Info("{Ack}", ack);

            if (ack > 0) return;

            if (initialized)
            {
                Log.Info("Become Ready");
                Become(Ready);
            }
            else
                Self.Tell(new Initialize());
        }

        /// <summary>
        /// Trigger the initialization process
        /// </summary>
        private class Initialize
        {}
    }
}
