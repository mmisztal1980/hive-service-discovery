using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Hive.Cluster.Messages
{
    public class ServiceDiscovery
    {
        public class InstanceStarted
        {
            public InstanceStarted(string serviceName, Uri serviceEndpoint, Address clusterAddress)
            {                
                ServiceName = serviceName;
                ServiceEndpoint = serviceEndpoint;
                ClusterAddress = clusterAddress;
            }

            /// <summary>
            /// Logical name of the service
            /// </summary>
            public string ServiceName { get; }

            /// <summary>
            /// Internal service endpoint
            /// </summary>
            public Uri ServiceEndpoint { get; }

            /// <summary>
            /// The cluster address of the sender node
            /// </summary>
            public Address ClusterAddress { get; }
        }

        public class ACK
        {}

        /// <summary>
        /// Request service information from leader
        /// </summary>
        public class GetServices
        {}

        /// <summary>
        /// Leader response for service information request
        /// </summary>
        public class SetServices
        {
            public IList<Service> Services { get; } = new List<Service>();
        }
    }
}
