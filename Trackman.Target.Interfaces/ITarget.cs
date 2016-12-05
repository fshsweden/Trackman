using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace Trackman.Target.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ITarget : IActor
    {
        /// <summary>
        /// Get Latest Location of this Target (actor)
        /// </summary>
        /// <returns></returns>
        Task<KeyValuePair<float, float>> GetLatestLocation();

        /// <summary>
        /// Set Location of this Target (actor)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task SetLocation(DateTime timestamp, float latitude, float longitude);
    }
}
