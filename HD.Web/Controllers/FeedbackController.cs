using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HD.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class FeedbackController : ControllerBase
    {
        IServiceFeedback sf;
        IServiceComplaint sc;

        public FeedbackController(IServiceFeedback sf, IServiceComplaint sc)
        {
            this.sf = sf;
            this.sc = sc;
            
        }

        [HttpPost("complaintId")]
        [Authorize(Roles = "Client")]
        public IActionResult AddFeedback (int complaintId, [FromBody] FeedbackRequest req)
        {
            var clientIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(clientIdClaim))
                return Unauthorized("Cannot get user ID.");

            var clientId = int.Parse(clientIdClaim);

            //Vérifier que la réclamation appartient au client 
            var complaint = sc.Get(c => c.ComplaintId == complaintId && c.ClientFK == clientId);
            if (complaint == null) return Forbid("Claim not found or doesn't belong to this client.");

            if (complaint.ComplaintState != State.Closed)
                return BadRequest("a feedback can only be attributed for a closed claim.");

            //Ajout du feedback
            sf.AddFeedback(complaintId,req.Rating,req.Message);

            return Ok(new { message = "Feedback successfully added" });

        }

        // Récupérer feedback d’une réclamation
        [HttpGet("{complaintId}")]
        public IActionResult GetFeedback(int complaintId)
        {
            var feedback = sf.GetFeedbackByComplaint(complaintId);
            if (feedback == null) return NotFound("Feedback not found.");

            return Ok(feedback);
        }
    }

    // DTOs
    public class FeedbackRequest
    {
        public int Rating { get; set; } // 1 à 5
        public string Message { get; set; }
    }
}
