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

        public AgentController(IServiceAdmin sa, IServiceAgentClaimLog sacl)
        {
            this.sa = sa;
            this.sacl = sacl;
        }

        //Gestion des comptes des Admins
        [HttpGet("admins/by-status")]
        public IActionResult GetAdminsByAccountStatus([FromQuery] AccountStatus status)
        {
            var admins = sa.GetAdminsByStatus(status);
            return Ok(admins);

        }

        [HttpPut("admin/{adminId}/status")]
        public IActionResult UpdatedAdminAccountStatus(int adminId, [FromQuery] UpdateAdminStatusRequest req)
        {
            var agentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            sa.UpdateAccountStatus(adminId,req.NewStatus, agentId);

            return Ok($" the account status of the admin {adminId} is changed to {req.NewStatus}");
        }

        //Affectation des réclamations
        [HttpPost("assign/{complaintId}/{adminId}")]
        public IActionResult AssignComplaintToAdmin(int complaintId, int adminId)
        {
            var agentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            sacl.AssignComplaintToAdmin(complaintId,agentId,adminId);

            return Ok($"Complaint with id {complaintId} is successfully assigned to the Admin with the id {adminId}");
        }

        [HttpGet("assignments/by-admin/{adminId}")]
        public IActionResult GetAssignmentsByAdmin(int adminId)
        {
            var assignments = sacl.GetAssignmentsByAdmin(adminId);
            return Ok(assignments);
        }

        [HttpGet("assignments/by-complaint/{complaintId}")]
        public IActionResult GetAssignmentsByComplaint(int complaintId)
        {
            var assignments = sacl.GetAssignmentsByComplaint(complaintId);
            return Ok(assignments);
        }

    }


    //Les DTOs
    public class UpdateAdminStatusRequest
    {
        public AccountStatus NewStatus { get; set; }
    }



}   





