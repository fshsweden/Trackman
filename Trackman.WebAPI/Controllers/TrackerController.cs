using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Owin.Logging;
using System.Threading.Tasks;
using Trackman.Tracker.Interfaces;

namespace Trackman.WebAPI.Controllers
{
    [ServiceRequestActionFilter]
    public class TrackerController : ApiController
    {
        [HttpGet]
        [Route("")]
        public string Index()
        {
            return "Welcome to Trackman 1.0.0";
        }

        [HttpPost]
        [Route("locations")]
        public async Task<bool> Log(Location location)
        {
            var reporter = TrackerConnectionFactory.CreateLocationReporter();
            await reporter.ReportLocation(location);
            return true;
        }

        [HttpGet]
        [Route("Target/{TargetId}/lastseen")]
        public async Task<DateTime?> LastSeen(Guid TargetId)         // DateTime?
        {
            // Create a ServiceProxy
            var viewer = TrackerConnectionFactory.CreateLocationViewer();
            return await viewer.GetLastReportTime(TargetId);
        }

        [HttpGet]
        [Route("Target/{TargetId}/lastlocation")]
        public async Task<object> LastLocation(Guid TargetId)         // DateTime?
        {
            var viewer = TrackerConnectionFactory.CreateLocationViewer();
            var location = await viewer.GetLastTargetLocation(TargetId);
            if (location == null)
                return null;

            return new { Latitude = location.Value.Key, Longitude = location.Value.Value };
        }


        /*
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
        */
    }
}
