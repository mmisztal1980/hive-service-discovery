using System;
using Akka.Actor;
using Akka.Configuration;

namespace Hive.Cluster
{
    public abstract class MicroserviceStartup
    {
        public abstract Func<Config, string> ActorSystemName { get; }

        /// <summary>
        /// Reads the Akka.NET ActorSystem Configuration
        /// </summary>
        /// <returns>Akka.NET Hocon Config</returns>
        public virtual Config ConfigureActorSystem()
        {
            return ConfigurationFactory.Load();
        }

        /// <summary>
        /// Configure and start the actors for the provided ActorSystem
        /// </summary>
        /// <param name="system"></param>
        public virtual void StartActors(ActorSystem system)
        {            
        }

        /// <summary>
        /// Configure DI
        /// </summary>
        public virtual void Configure()
        { }
    }
}
