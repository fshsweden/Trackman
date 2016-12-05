using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Trackman.Target.Interfaces;

namespace Trackman.Target
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class Target : Actor, ITarget
    {

        /*
         * 
         * 
         * 
         */
        [DataContract]
        internal sealed class LocationAtTime
        {
            public DateTime Timestamp { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }

        [DataContract]
        internal sealed class TargetState
        {
            [DataMember]
            public List<LocationAtTime> LocationHistory { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of Target
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public Target(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public async Task<KeyValuePair<float, float>> GetLatestLocation()
        {
            ActorEventSource.Current.ActorMessage(this,"Target::GetLatestLocation()");

            var state = await StateManager.GetStateAsync<TargetState>("State");
            var location = state.LocationHistory.OrderByDescending(x => x.Timestamp).Select(x =>
                new KeyValuePair<float, float>(x.Latitude, x.Longitude)
            ).FirstOrDefault();

            return location;
        }

        public async Task SetLocation(DateTime timestamp, float latitude, float longitude)
        {
            ActorEventSource.Current.ActorMessage(this, "Target::SetLocation()");

            var state = await StateManager.GetStateAsync<TargetState>("State");
            state.LocationHistory.Add(new LocationAtTime() { Timestamp = timestamp, Latitude = latitude, Longitude = longitude });

            await StateManager.AddOrUpdateStateAsync("State", state, (s, actorState) => state);
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated (Target::OnActivateAsync)");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            // return this.StateManager.TryAddStateAsync("count", 0);

            var state = await StateManager.TryGetStateAsync<TargetState>("State");
            if (!state.HasValue)
                await StateManager.AddStateAsync("State", new TargetState { LocationHistory = new List<LocationAtTime>() });
        }



        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        // Task<int> ITarget.GetCountAsync()
        // {
        //     return this.StateManager.GetStateAsync<int>("count");
        // }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        // Task ITarget.SetCountAsync(int count)
        // {
        // Requests are not guaranteed to be processed in order nor at most once.
        // The update function here verifies that the incoming count is greater than the current count to preserve order.
        //     return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value);
        // }
    }
}
