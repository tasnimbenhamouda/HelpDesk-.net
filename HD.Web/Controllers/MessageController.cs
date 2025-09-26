using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HD.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        IServiceMessage sm;
        IServiceComplaint sc;

        public MessageController(IServiceComplaint sc, IServiceMessage sm)
        {
            this.sc = sc;
            this.sm = sm;
        }

        //Envoi d'un message dans une réclamation (Client ou admin)
        [HttpPost("compaintId")]
        public IActionResult SendMessage (int complaintId, [FromBody] MessageRequest req)
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Cannot get user ID.");

            var userId = int.Parse(userIdClaim);

            var role = User.FindFirstValue(ClaimTypes.Role);

            // Vérifier si l’utilisateur a bien le droit d’envoyer un message pour cette réclamation
            var complaint = sc.Get(c => c.ComplaintId == complaintId);
            if (complaint == null) return NotFound("Complaint not found.");

            if (role == "Client" && complaint.ClientFK != userId)
                return Forbid("Claim doesn't belong to this client.");

            if (role == "Admin" && !complaint.AgentClaimLogs.Any(l => l.AdminFK == userId && l.Affected==true))
                return Forbid("This claim is not assigned to this Admin.");

            // Enregistrer message
            sm.SendMessage(complaintId, userId, req.Content,
                role == "Client" ? TypeSender.Client : TypeSender.Admin);

            return Ok(new { message = "Message Successfully sent" });

        }

        // Récupérer tous les messages d’une réclamation
        [HttpGet("{complaintId}")]
        public IActionResult GetMessages(int complaintId)
        {
            var messages = sm.GetMessagesByComplaint(complaintId);

            var messageDtos = messages.Select(m => new MessageDto
            {
                SenderId = m.SenderId,
                Content = m.Content,
                TypeSender = m.TypeSender,
                SendDate = m.SendDate,
                ComplaintFK = m.ComplaintFK
            });

            return Ok(messageDtos);
        }



    }
    //DTO
    public class MessageRequest
    {
        public string Content { get; set; }
    }

    public class MessageDto
    {
        public int SenderId { get; set; }
        public string Content { get; set; }
        public TypeSender TypeSender { get; set; }
        public DateTime SendDate { get; set; }
        public int ComplaintFK { get; set; }
    }

}
