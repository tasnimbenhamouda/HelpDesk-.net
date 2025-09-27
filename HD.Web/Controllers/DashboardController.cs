using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using HD.ApplicationCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HD.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        IServiceComplaint isc;
        IServiceAgentClaimLog isacl;
        IServiceFeedback sf;

        public DashboardController(IServiceComplaint isc, IServiceAgentClaimLog isacl, IServiceFeedback sf)
        {
            this.isc = isc;
            this.isacl = isacl;
            this.sf = sf;
        }

        [HttpGet("total-complaints")]
        public IActionResult GetTotalComplaints()
        {
            var result = isc.GetTotalComplaints();
            return Ok(result);
        }

        [HttpGet("complaints-by-admin")]
        public IActionResult GetComplaintsCountByAdmin()
        {
            var result = isacl.GetComplaintsCountByAdmin();
            return Ok(result);
        }

        [HttpGet("avg-resolution-time")]
        public IActionResult GetAverageResolutionTime()
        {
            var result = isc.GetAverageResolutionTime();
            return Ok(result);
        }

        [HttpGet("avg-resolution-time-by-admin")]
        public IActionResult GetAverageResolutionTimeByAdmin()
        {
            var result = isacl.GetAverageResolutionTimeByAdmin();
            return Ok(result);
        }

        [HttpGet("complaints-by-feature")]
        public IActionResult GetComplaintsCountByFeature()
        {
            var result = isc.GetComplaintsCountByFeature();
            return Ok(result);
        }

        [HttpGet("complaints-by-type")]
        public IActionResult GetComplaintsCountByType()
        {
            var result = isc.GetComplaintsCountByType();
            return Ok(result);
        }

        [HttpGet("complaints-by-state")]
        public IActionResult GetComplaintsCountByState()
        {
            var result = isc.GetComplaintsCountByState();
            return Ok(result);
        }

        [HttpGet("complaints-by-status")]
        public IActionResult GetComplaintsCountByStatus()
        {
            var result = isc.GetComplaintsCountByStatus();
            return Ok(result);
        }

        [HttpGet("avg-client-feedback")]
        public IActionResult GetAverageClientFeedback()
        {
            var result = sf.GetAverageClientFeedback();
            return Ok(result);
        }

        [HttpGet("get-statistics")]
        public IActionResult GetStatistics()
        {
            var totalComplaints = isc.GetTotalComplaints();
            var complaintsByAdmin = isacl.GetComplaintsCountByAdmin();
            var averageResolutionTime = isc.GetAverageResolutionTime();
            var averageResolutionTimeByAdmin = isacl.GetAverageResolutionTimeByAdmin();
            var complaintsByFeature = isc.GetComplaintsCountByFeature();
            var complaintsByType = isc.GetComplaintsCountByType();
            var complaintsByState = isc.GetComplaintsCountByState();
            var complaintsByStatus = isc.GetComplaintsCountByStatus();
            var averageClientFeedback = sf.GetAverageClientFeedback();

            var result = new DashboardDto
            {
                TotalComplaints = totalComplaints,
                ComplaintsByAdmin = complaintsByAdmin,
                AverageResolutionTime = averageResolutionTime,
                AverageResolutionTimeByAdmin = averageResolutionTimeByAdmin,
                ComplaintsByFeature = complaintsByFeature,
                ComplaintsByType = complaintsByType,
                ComplaintsByState = complaintsByState,
                ComplaintsByStatus = complaintsByStatus,
                AverageClientFeedback = averageClientFeedback
            };
            return Ok(result);
        }
    }

    public class DashboardDto
    {
        public int TotalComplaints { get; set; }
        public Dictionary<string, int> ComplaintsByAdmin { get; set; }
        public double AverageResolutionTime { get; set; }
        public Dictionary<string,double> AverageResolutionTimeByAdmin { get; set; }
        public Dictionary<int,int> ComplaintsByFeature { get; set; }
        public Dictionary<ComplaintType, int> ComplaintsByType { get; set; }
        public Dictionary<State, int> ComplaintsByState { get; set; }
        public Dictionary<Status, int> ComplaintsByStatus { get; set; }
        public double AverageClientFeedback { get; set; }
    }
}
