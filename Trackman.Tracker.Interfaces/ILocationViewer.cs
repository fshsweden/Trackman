using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Trackman.Tracker.Interfaces
{
    public interface ILocationViewer : IService
    {
        Task<KeyValuePair<float, float>?> GetLastTargetLocation(Guid TargetId);
        Task<DateTime?> GetLastReportTime(Guid TargetId);
    }
}
