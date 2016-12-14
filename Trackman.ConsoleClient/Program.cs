using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Trackman.Target.Interfaces;

namespace Trackman.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var target = ActorProxy.Create<ITarget>(ActorId.CreateRandom(), "fabric:/Trackman/TargetActorService");

            target.SetLocation()

        }
    }
}
