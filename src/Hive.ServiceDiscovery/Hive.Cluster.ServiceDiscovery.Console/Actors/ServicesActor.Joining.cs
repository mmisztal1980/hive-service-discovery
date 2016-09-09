using Akka.Cluster;

namespace Hive.Cluster.ServiceDiscovery.Actors
{
    public partial class ServicesActor
    {
        private void Joining()
        {
            Log.Info("Joining");

            Receive<ClusterEvent.MemberUp>(
                x => CanHandleMemberUp(x), 
                x => OnMemberUp(x));
        }

        /// <summary>
        /// Execute only when the current node joins the cluster
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>true if the recently joined member's address equals to address of current node</returns>
        private bool CanHandleMemberUp(ClusterEvent.MemberUp msg)
        {
            return Cluster.SelfAddress.Equals(msg.Member.Address);
        }

        /// <summary>
        /// When the current node joins the Akka.NET cluster - become Initializing
        /// </summary>
        /// <param name="msg"></param>
        private void OnMemberUp(ClusterEvent.MemberUp msg)
        {
            Log.Info("{@Event}", msg);
            Log.Info("Become Initializing");
            Become(Initializing);
        }
    }
}
