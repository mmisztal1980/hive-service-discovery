# hive-service-discovery

# Gettings started:

## Lighthouse

Provide a single lighthouse instance with the following parameters:
- actor system name: 'hive'
- IP: 127.0.0.1
- port: 50000
- set auto-down to 30'ish secs 

## ServiceDiscovery

The minimum amount of nodes required for the cluster is: 5
*akka.cluster.role.servicediscovery.min-nr-of-members = 5*

## Logging

I recommend having SEQ installed, by default the console app will produce logs to the 
Literate console sink. 

In order to enable SEQ logging:
- Install SEQ from this [site](http://getseq.net)
- Uncomment the appsettings in app.config

# Reproduction

The nodes are supposed to change their state in this way:
**Joining -> Initializing -> Ready**

After launching Lighthouse, activate 5 nodes of the console app.
1 of the nodes (role leader) will become Ready immediately and is supposed to listen for
```GetServices``` events from nodes which are still initializing.

For some reason the ```Receive<GetEvents>``` handler is not firing and the 4 remaining nodes are stuck
in the ```Initializing``` state.