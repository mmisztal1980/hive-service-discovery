using System;
using Akka.Actor;

namespace Hive.Cluster.Messages
{
    public class Service
    {
        public Service(string name, Uri endpoint, Address clusterAddress)
        {
            Name = name;
            Endpoint = endpoint;
            ClusterAddress = clusterAddress;
        }

        public Address ClusterAddress { get; }
        public Uri Endpoint { get; }
        public string Name { get; }
    }
}
