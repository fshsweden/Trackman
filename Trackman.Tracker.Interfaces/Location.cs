using System;

namespace Trackman.Tracker.Interfaces
{
    public class Location
    {
        public Guid TargetId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}