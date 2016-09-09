using System.Linq;
using Akka.Cluster;
using Akka.Util.Internal;
using Hive.Cluster.Messages;
using SvcDiscovery = Hive.Cluster.Messages.ServiceDiscovery;
namespace Hive.Cluster.ServiceDiscovery.Actors
{
    public partial class ServicesActor
    {
        private void Ready()
        {
            Log.Info("Ready (RoleLeader: {roleLeader})", IsRoleLeader);

            Receive<SvcDiscovery.GetServices>(x => OnGetServices(x), (x) => true);

            Receive<SvcDiscovery.InstanceStarted>(CanHandleInstanceStarted, x => OnInstanceStarted(x));

            Receive<CommitService>(CanHandleCommitService, x => OnServiceCommit(x));
            
            Receive<ClusterEvent.UnreachableMember>(x => OnUnreachableMember(x));     
                        
            Log.Info("Ready (RoleLeader: {roleLeader}) all handlers registered", IsRoleLeader);

            Self.Tell(new SvcDiscovery.GetServices(), Self);

        }

        #region UnreachableMember

        private void OnUnreachableMember(ClusterEvent.UnreachableMember msg)
        {
            Log.Info("{@Event}", msg);

            if (Cluster.SelfAddress.Equals(msg.Member.Address))
            {
                Log.Info("Become Unreachable");
                Become(Unreachable);
            }
            else
            {

                // handle other member being unreachable
            }
        }

        #endregion

        private class CommitService
        {
            public CommitService(Service service)
            {
                Service = service;
            }

            public Service Service { get; }
        }

        #region InstanceStarted

        private bool CanHandleInstanceStarted(SvcDiscovery.InstanceStarted msg)
        {
            return IsRoleLeader;
        }

        private void OnInstanceStarted(SvcDiscovery.InstanceStarted msg)
        {
            Log.Info("{@Event}", msg);

            var svc = Services.SingleOrDefault(x => x.ClusterAddress.Equals(msg.ClusterAddress));
            var newService = svc != null;

            svc = new Service(msg.ServiceName, msg.ServiceEndpoint, msg.ClusterAddress);

            if (newService) Services.Add(svc);

            ClusterBroadcast.Tell(new CommitService(svc), Self);

            //todo: Notify API Gateways
        }

        #endregion

        #region GetServices

        private bool CanHandleGetServices(SvcDiscovery.GetServices msg)
        {
            Log.Info("CanHandle {@Event} ? : {IsRoleLeader}", msg, IsRoleLeader);

            return IsRoleLeader;
        }

        private void OnGetServices(SvcDiscovery.GetServices msg)
        {
            Log.Info("{@Event}", msg);

            var response = new SvcDiscovery.SetServices();
            Services.ForEach(x => response.Services.Add(x));

            Sender.Tell(response, Self);
        }

        #endregion

        #region CommitService

        private bool CanHandleCommitService(CommitService msg)
        {
            return !IsRoleLeader;
        }

        private void OnServiceCommit(CommitService msg)
        {
            Services.Add(msg.Service);
        }

        #endregion
    }
}
