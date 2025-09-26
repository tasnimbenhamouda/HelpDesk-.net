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
    }
}
