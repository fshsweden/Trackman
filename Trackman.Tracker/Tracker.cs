using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Trackman.Tracker.Interfaces;
using Trackman.Target.Interfaces;

namespace Trackman.Tracker
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Tracker : StatefulService, ILocationReporter, ILocationViewer
    {
        public Tracker(StatefulServiceContext context) : base(context)
        {
            
        }

        // ILocationViewer (an IService)
        public async Task<DateTime?> GetLastReportTime(Guid TargetId)
        {
            logMsg(string.Format("TRACKER - GetLastReportTime called with {0} ", TargetId));

            using (var tx = StateManager.CreateTransaction())
            {
                var timestamps = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, DateTime>>("timestamps");

                var timestamp = await timestamps.TryGetValueAsync(tx, TargetId);
                await tx.CommitAsync();

                return timestamp.HasValue ? (DateTime?)timestamp.Value : null;
            }
        }

        public async Task<KeyValuePair<float, float>?> GetLastTargetLocation(Guid TargetId)
        {
            logMsg(string.Format("TRACKER - GetLastTargetLocation called with {0} ", TargetId));

            using (var tx = StateManager.CreateTransaction())
            {
                var TargetIds = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, ActorId>>("TargetIds");

                var TargetActorId = await TargetIds.TryGetValueAsync(tx, TargetId);
                if (!TargetActorId.HasValue)
                    return null;

                // Create an ActorProxy
                var Target = TargetConnectionFactory.GetTarget(TargetActorId.Value);
                return await Target.GetLatestLocation();
            }
        }

        // ILocationReporter  (an IService)
        public async Task ReportLocation(Location location)
        {
            logMsg(string.Format("TRACKER - ReportLocation called with {0} {1} {2} ", location.TargetId, location.Longitude, location.Latitude));

            using (var tx = StateManager.CreateTransaction())
            {
                var timestamps = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, DateTime>>("timestamps");
                var TargetIds = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, ActorId>>("TargetIds");

                var timestamp = DateTime.UtcNow;

                // Update Target (add TargetId with a random Actor ID if we dont already have it)
                var TargetActorId = await TargetIds.GetOrAddAsync(tx, location.TargetId, ActorId.CreateRandom());

                // Create an ActorProxy
                await TargetConnectionFactory.GetTarget(TargetActorId).SetLocation(timestamp, location.Latitude, location.Longitude);

                // Update service with new timestamp
                await timestamps.AddOrUpdateAsync(tx, location.TargetId, DateTime.UtcNow, (guid, time) => timestamp);
                await tx.CommitAsync();
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            /*
             * Whats the difference here???
             */
            // return new ServiceReplicaListener[0];

            // see: https://docs.microsoft.com/en-in/azure/service-fabric/service-fabric-reliable-services-communication-remoting
            return new[] { new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context)) };

            /*return new[] {
                new ServiceReplicaListener(
                    initParams => new ServiceReplicaListener()Listener<Tracker>(initParams, this)
                )
            };*/
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                /*
                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }
                */

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }



        private void logMsg(string msg)
        {
            ServiceEventSource.Current.ServiceMessage(this.Context, msg);

        }
    }
}
