using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace HD.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        IServiceComplaint sc;
        IServiceAgentClaimLog sacl;
        public ComplaintController(IServiceComplaint sc, IServiceAgentClaimLog sacl)
        {
            this.sc = sc;
            this.sacl = sacl;
        }


        //Endpoints Client
        [HttpPost("Create")]
        [Authorize(Roles="Client")]
        public IActionResult CreateComplaint([FromBody] CreateComplaintRequest request)
        {
            var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var filePaths = new List<string>();

            if (request.Files != null && request.Files.Any())
            {
                foreach (var file in request.Files)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    filePaths.Add($"/uploads/{uniqueFileName}");
                }
            }

            sc.CreateComplaint(
                clientId,
                request.Title,
                request.Description,
                request.ComplaintType,
                filePaths,
                request.FeatureId
            );

            return Ok("Réclamation créée avec succès.");
        }

        [HttpPut("update/{complaintId}")]
        [Authorize(Roles = "Client")]
        public IActionResult UpdateComplaint(int complaintId, [FromBody] UpdateComplaintRequest request)
        {
            var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var filePaths = new List<string>();

            if (request.Files != null && request.Files.Any())
            {
                foreach (var file in request.Files)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    filePaths.Add($"/uploads/{uniqueFileName}");
                }
            }

            sc.UpdateComplaintByClient(
                complaintId,
                clientId,
                request.Title,
                request.Description,
                request.ComplaintType,
                filePaths,
                request.FeatureId
            );

            return Ok("Réclamation mise à jour avec succès.");
        }

        [HttpDelete("delete/{complaintId}")]
        [Authorize(Roles = "Client")]
        public IActionResult DeleteComplaint(int complaintId)
        {
            var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            sc.DeletePendingComplaint(complaintId, clientId);

            return Ok("Réclamation supprimée avec succès.");
        }

        [HttpGet("my-complaints")]
        [Authorize(Roles = "Client")]
        public IActionResult GetMyComplaints()
        {
            var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var complaints = sc.GetComplaintsByClientId(clientId);
            return Ok(complaints);
        }

        [HttpGet("client/{complaintId}/details")]
        [Authorize(Roles = "Client")]
        public IActionResult GetComplaintDetailsForClient(int complaintId)
        {
            var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var complaint = sc.GetComplaintDetails(complaintId, clientId);

            if (complaint == null)
                return NotFound("Réclamation introuvable ou n'appartient pas à ce client.");

            return Ok(new
            {
                complaint.ComplaintId,
                complaint.Title,
                complaint.Description,
                complaint.ComplaintType,
                complaint.SubmissionDate,
                complaint.ComplaintState,
                complaint.ComplaintStatus,
                Feature = complaint.Feature?.Name,
                Files = complaint.ComplaintFiles?.Select(f => f.FilePath).ToList()
            });
        }


        [HttpPost("validate-closure/{complaintId}")]
        [Authorize(Roles = "Client")]
        public IActionResult ValidateClosure(int complaintId, [FromBody] bool resolved)
        {
            var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            sc.ValidateClosure(complaintId, clientId, resolved);

            return Ok("Validation de la clôture effectuée.");
        }


        [HttpGet("client/filter")]
        [Authorize(Roles = "Client")]
        public IActionResult FilterClientComplaints([FromQuery] ComplaintFilterRequest request)
        {
            var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            IEnumerable<Complaint> complaints = sc.GetComplaintsByClientId(clientId);

            if (request.State.HasValue)
                complaints = complaints.Where(c => c.ComplaintState == request.State.Value);

            if (request.Status.HasValue)
                complaints = complaints.Where(c => c.ComplaintStatus == request.Status.Value);

            if (request.Type.HasValue)
                complaints = complaints.Where(c => c.ComplaintType == request.Type.Value);

            if (!string.IsNullOrWhiteSpace(request.FeatureName))
                complaints = complaints.Where(c => c.Feature != null &&
                                                   c.Feature.Name.ToLower() == request.FeatureName.ToLower());

            if (request.SubmissionDate.HasValue)
                complaints = complaints.Where(c => c.SubmissionDate.Date == request.SubmissionDate.Value.Date);

            return Ok(complaints);
        }


        //Endpoints Admin 

        [HttpGet("assigned")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAssignedComplaints()
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var complaints = sc.GetComplaintsByAdmin(adminId);
            return Ok(complaints);
        }

        [HttpGet("admin/{complaintId}/details")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetComplaintDetailsForAdmin(int complaintId)
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var complaint = sc.GetComplaintDetailsByAdmin(adminId, complaintId);

            if (complaint == null)
                return NotFound("Réclamation introuvable ou non assignée à cet administrateur.");

            return Ok(new
            {
                complaint.ComplaintId,
                complaint.Title,
                complaint.Description,
                complaint.ComplaintType,
                complaint.SubmissionDate,
                complaint.ProcessedDate,
                complaint.ComplaintState,
                complaint.ComplaintStatus,
                Feature = complaint.Feature?.Name,
                ClientName = complaint.Client?.clientName,
                Files = complaint.ComplaintFiles?.Select(f => f.FilePath).ToList(),
                AssignedAdmins = complaint.AgentClaimLogs?.Select(l => new
                {
                    l.AdminFK,
                    AdminName = l.Admin?.Name,
                    l.AffectedDate,
                    l.ProcessedDate
                })
            });
        }


        [HttpPut("update-state/{complaintId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateComplaintState(int complaintId, [FromBody] UpdateComplaintStateRequest request)
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            sc.UpdateComplaintState(adminId, complaintId, request.NewState);

            return Ok("État de la réclamation mis à jour avec succès.");
        }


        [HttpPost("rollback/{complaintId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult RollbackComplaint(int complaintId)
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            sc.RollbackComplaintToAgent(adminId, complaintId);

            return Ok("Réclamation rollbackée à l’agent.");
        }

        [HttpGet("admin/filter")]
        [Authorize(Roles ="Admin")]
        public IActionResult FilterAdminComplaints([FromQuery] ComplaintFilterRequest req)
        {

            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            IEnumerable<Complaint> complaints = sc.GetComplaintsByAdmin(adminId);

            if (req.State.HasValue)
                complaints = complaints.Where(c=>c.ComplaintState == req.State.Value);

            if (req.Status.HasValue)
                complaints = complaints.Where(c=>c.ComplaintStatus == req.Status.Value);

            if (req.Type.HasValue)
                complaints = complaints.Where(c=>c.ComplaintType == req.Type.Value);

            if (!string.IsNullOrWhiteSpace(req.FeatureName))
            complaints = complaints.Where(c => c.Feature != null &&
                                                    c.Feature.Name.ToLower() == req.FeatureName.ToLower());

            if (req.SubmissionDate.HasValue)
                complaints = complaints.Where(c=>c.SubmissionDate.Date == req.SubmissionDate.Value.Date);

            if (!string.IsNullOrWhiteSpace(req.Name))
            complaints = complaints.Where(c=>c.Client != null &&
                                                    c.Client.clientName.ToLower() == req.Name.ToLower());

            return Ok(complaints);

        }

        //Endpoints Agent 
        [HttpGet("all")]
        [Authorize(Roles = "Agent")]
        public IActionResult GetAllComplaints()
        {
            var complaints = sc.GetAll();
            return Ok(complaints);
        }

        [HttpPost("assign/{complaintId}/{adminId}")]
        [Authorize(Roles = "Agent")]
        public IActionResult AssignComplaint(int complaintId, int adminId)
        {
            var agentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            sacl.AssignComplaintToAdmin(complaintId, agentId, adminId);

            return Ok("Réclamation assignée à un admin.");
        }

        [HttpGet("agent/filter")]
        [Authorize(Roles = "Agent")]
        public IActionResult FilterAgentComplaints([FromQuery] ComplaintFilterRequest req)
        {
            var agentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            IEnumerable<Complaint> complaints = sc.GetAll();

            if(req.State.HasValue)
                complaints = complaints.Where(c=>c.ComplaintState == req.State.Value);

            if (req.Status.HasValue)
                complaints = complaints.Where(c=>c.ComplaintStatus == req.Status.Value); 

            if(req.Type.HasValue)
                complaints = complaints.Where(c=>c.ComplaintType == req.Type.Value);

            if (req.SubmissionDate.HasValue)
                complaints = complaints.Where(c => c.SubmissionDate.Date == req.SubmissionDate.Value.Date);

            if (req.ProcessedDate.HasValue)
                complaints = complaints.Where(c => c.ProcessedDate.Date == req.ProcessedDate.Value.Date);

            if (!string.IsNullOrWhiteSpace(req.FeatureName))
                complaints = complaints.Where(c => c.Feature != null &&
                                                        c.Feature.Name.ToLower() == req.FeatureName.ToLower());

            if (!string.IsNullOrWhiteSpace(req.Name))
                complaints = complaints.Where(c => c.Client != null &&
                                                        c.Client.clientName.ToLower() == req.Name.ToLower());

            if (!string.IsNullOrWhiteSpace(req.AdminName))
                complaints = complaints.Where(c => c.AgentClaimLogs.Any(a =>
                                                       a.Admin != null && a.Admin.Name.ToLower() == req.AdminName.ToLower()));

            return Ok (complaints);
             
        }

    }

    //Les Classes DTO

    public class CreateComplaintRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ComplaintType ComplaintType { get; set; }
        public List<IFormFile>? Files { get; set; }
        public int FeatureId { get; set; }
    }

    public class UpdateComplaintRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ComplaintType ComplaintType { get; set; }
        public List<IFormFile>? Files { get; set; }
        public string? FilePath { get; set; }
        public int FeatureId { get; set; }
    }

    public class UpdateComplaintStateRequest
    {
        public State NewState { get; set; }
    }

    public class ComplaintFilterRequest
    {
        public State? State { get; set; }
        public Status? Status { get; set; }
        public ComplaintType? Type { get; set; }
        public string? FeatureName { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string? Name { get; set; }
        public string? AdminName { get; set; }
    }

}
