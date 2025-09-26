using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HD.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Agent")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        IServiceAdmin sa;
        IServiceAgentClaimLog sacl;
        IServiceAgent isa;

        public AgentController(IServiceAdmin sa, IServiceAgentClaimLog sacl, IServiceAgent isa)
        {
            this.sa = sa;
            this.sacl = sacl;
            this.isa = isa;
        }

        //Gestion des comptes des Admins
        [HttpGet("admins/by-status")]
        public IActionResult GetAdminsByAccountStatus([FromQuery] AccountStatus status)
        {
            var agentIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(agentIdClaim))
                return Unauthorized("Cannot get user ID.");

            var agentId = int.Parse(agentIdClaim);

            var admins = sa.GetAdminsByStatus(status);
            return Ok(admins);

        }

        [HttpPut("admin/{adminId}/status")]
        public IActionResult UpdatedAdminAccountStatus(int adminId, [FromQuery] UpdateAdminStatusRequest req)
        {
            var agentIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(agentIdClaim))
                return Unauthorized("Cannot get user ID.");

            var agentId = int.Parse(agentIdClaim);

            sa.UpdateAccountStatus(adminId,req.NewStatus, agentId);

            return Ok($" the account status of the admin {adminId} is changed to {req.NewStatus}");
        }

        //Affectation des réclamations
        [HttpPost("assign/{complaintId}/{adminId}")]
        public IActionResult AssignComplaintToAdmin(int complaintId, int adminId)
        {
            var agentIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(agentIdClaim))
                return Unauthorized("Cannot get user ID.");

            var agentId = int.Parse(agentIdClaim);

            sacl.AssignComplaintToAdmin(complaintId,agentId,adminId);

            return Ok($"Complaint with id {complaintId} is successfully assigned to the Admin with the id {adminId}");
        }

        [HttpGet("assignments/by-admin/{adminId}")]
        public IActionResult GetAssignmentsByAdmin(int adminId)
        {
            var agentIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(agentIdClaim))
                return Unauthorized("Cannot get user ID.");

            var agentId = int.Parse(agentIdClaim);

            var assignments = sacl.GetAssignmentsByAdmin(adminId);
            return Ok(assignments);
        }

        [HttpGet("assignments/by-complaint/{complaintId}")]
        public IActionResult GetAssignmentsByComplaint(int complaintId)
        {
            var agentIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(agentIdClaim))
                return Unauthorized("Cannot get user ID.");

            var agentId = int.Parse(agentIdClaim);

            var assignments = sacl.GetAssignmentsByComplaint(complaintId);
            return Ok(assignments);
        }

        [HttpGet("{agentId}/name")]
        public IActionResult GetAgentName(int agentId)
        {
            var name = isa.GetAgentName(agentId);

            if (name == null)
                return NotFound("Agent not found");

            return Ok(name);
        }

    }


    //Les DTOs
    public class UpdateAdminStatusRequest
    {
        public AccountStatus NewStatus { get; set; }
    }



}   





