using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;

namespace Trackman.Tracker.Interfaces
{
    public class TrackerConnectionFactory
    {
        private static readonly Uri LocationReporterServiceUrl = new Uri("fabric:/Trackman/Tracker");

        public static ILocationReporter CreateLocationReporter()
        {
            var partitionKey = new ServicePartitionKey(0);
            return ServiceProxy.Create<ILocationReporter>(LocationReporterServiceUrl, partitionKey);  // missing partition key
        }

        public static ILocationViewer CreateLocationViewer()
        {
            var partitionKey = new ServicePartitionKey(0);
            return ServiceProxy.Create<ILocationViewer>(LocationReporterServiceUrl, partitionKey);  // missing partition key
        }
    }
}
