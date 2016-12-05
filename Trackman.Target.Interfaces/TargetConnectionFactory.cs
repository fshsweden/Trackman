using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace Trackman.Target.Interfaces
{
    //
    // Wrapper class for ActorProxy.Create
    //
    public static class TargetConnectionFactory
    {
        private static readonly Uri TargetServiceUrl = new Uri("fabric:/Trackman/TargetActorService");

        public static ITarget GetTarget(ActorId actorId)
        {
            return ActorProxy.Create<ITarget>(actorId, TargetServiceUrl);
        }
    }
}