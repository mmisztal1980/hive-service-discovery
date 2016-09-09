using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Akka.Logger.Serilog;
using Akka.Routing;
using Hive.Cluster.Messages;
using SvcDiscovery = Hive.Cluster.Messages.ServiceDiscovery;
namespace Hive.Cluster.ServiceDiscovery.Actors
{
    /// <summary>
    /// /user/services
    /// </summary>
    public partial class ServicesActor : ReceiveActor, IWithUnboundedStash
    {
        public const string ActorName = "services";
        private const string ClusterRole = "servicediscovery";
        private const string ClusterBroadcastRouterName = "broadcast";
        protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);
        protected IActorRef ClusterBroadcast;
        protected ILoggingAdapter Log = Context.GetLogger(new SerilogLogMessageFormatter());

        public ServicesActor()
        {

            Log.Info("Become Joining");
            Joining();
        }

        private IList<Service> Services { get; } = new List<Service>();

        public bool IsRoleLeader => Cluster.SelfAddress.Equals(Cluster.State.RoleLeader(ClusterRole));

        public IStash Stash { get; set; }

        protected override void PreStart()
        {
            Cluster.Subscribe(Self,
                typeof(ClusterEvent.IMemberEvent),
                typeof(ClusterEvent.IReachabilityEvent));

            ClusterBroadcast = Context.Child(ClusterBroadcastRouterName).Equals(ActorRefs.Nobody)
                ? Context.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), ClusterBroadcastRouterName)
                : Context.Child(ClusterBroadcastRouterName);
        }

        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);

            Log.Info("PostStop");
        }

        /// <summary>
        /// Message used internally by Services Actors to exchange information about their services
        /// </summary>
        private class GetServicesRequest
        {}
    }
}
