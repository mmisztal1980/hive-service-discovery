using Akka.Cluster;

namespace Hive.Cluster.ServiceDiscovery.Actors
{
    public partial class ServicesActor
    {
        private void Unreachable()
        {
            Log.Info("Unreachable");

            Services.Clear();

            Receive<ClusterEvent.ReachableMember>(x => OnReachableMember(x));
        }

        /// <summary>
        /// When the node becomes reachable again, force reinitialization
        /// </summary>
        /// <param name="msg"></param>
        private void OnReachableMember(ClusterEvent.ReachableMember msg)
        {
            Log.Info("{@Event}", msg);

            if (!Cluster.SelfAddress.Equals(msg.Member.Address)) return;

            Become(Initializing);
        }
    }
}
