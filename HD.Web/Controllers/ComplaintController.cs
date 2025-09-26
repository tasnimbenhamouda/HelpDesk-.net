using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using HD.ApplicationCore.Services;
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
        IServiceComplaintFiles scf;
        public ComplaintController(IServiceComplaint sc, IServiceAgentClaimLog sacl, IServiceComplaintFiles scf)
        {
            this.sc = sc;
            this.sacl = sacl;
            this.scf = scf;
        }


        //Endpoints Client
        [HttpPost("Create")]
        [Authorize(Roles="Client")]
        public IActionResult CreateComplaint([FromForm] CreateComplaintRequest request)
        {
            var clientIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(clientIdClaim))
                return Unauthorized("Cannot get user ID.");

            var clientId = int.Parse(clientIdClaim);

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

            return Ok("Claim successfully submitted.");
        }

        [HttpPut("update/{complaintId}")]
        [Authorize(Roles = "Client")]
        public IActionResult UpdateComplaint(int complaintId, [FromForm] UpdateComplaintRequest request)
        {
            var clientIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(clientIdClaim))
                return Unauthorized("Cannot get user ID.");

            var clientId = int.Parse(clientIdClaim);

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

            return Ok("Claim successfully updated.");
        }

        [HttpDelete("delete/{complaintId}")]
        [Authorize(Roles = "Client")]
        public IActionResult DeleteComplaint(int complaintId)
        {
            var clientIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(clientIdClaim))
                return Unauthorized("Cannot get user ID.");

            var clientId = int.Parse(clientIdClaim);

            sc.DeletePendingComplaint(complaintId, clientId);

            return Ok("Claim successfully deleted.");
        }

        [HttpGet("my-complaints")]
        [Authorize(Roles = "Client")]
        public IActionResult GetMyComplaints()
        {
            var clientIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(clientIdClaim))
                return Unauthorized("Cannot get user ID.");

            var clientId = int.Parse(clientIdClaim);

            // Service renvoie les entités Complaint
            var complaints = sc.GetComplaintsByClientId(clientId);

            // Ici, mapping vers DTO
            var complaintDtos = complaints.Select(c => new ComplaintDto
            {
                ComplaintId = c.ComplaintId,
                Title = c.Title,
                Description = c.Description,
                SubmissionDate = c.SubmissionDate,
                ProcessedDate = c.ProcessedDate,
                ComplaintState = c.ComplaintState,
                ComplaintStatus = c.ComplaintStatus,
                ComplaintType = c.ComplaintType,
                FeatureFK = c.FeatureFK,
                ClientFK = c.ClientFK
            });

            return Ok(complaintDtos);
        }


        [HttpGet("client/{complaintId}/details")]
        [Authorize(Roles = "Client")]
        public IActionResult GetComplaintDetailsForClient(int complaintId)
        {
            var clientIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(clientIdClaim))
                return Unauthorized("Cannot get user ID.");

            var clientId = int.Parse(clientIdClaim);

            var complaint = sc.GetComplaintDetails(complaintId, clientId);

            if (complaint == null)
                return NotFound("Calim not found, or doesn't belong to this client.");

            return Ok(new
            {
                complaint.ComplaintId,
                complaint.Title,
                complaint.Description,
                complaint.ComplaintType,
                complaint.SubmissionDate,
                complaint.ComplaintState,
                complaint.ComplaintStatus,
                complaint.FeatureFK,
                complaint.ClientFK,
                Files = complaint.ComplaintFiles?.Select(f => f.FilePath).ToList()
            });
        }


        [HttpPost("validate-closure/{complaintId}")]
        [Authorize(Roles = "Client")]
        public IActionResult ValidateClosure(int complaintId, [FromBody] bool resolved)
        {
            var clientIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(clientIdClaim))
                return Unauthorized("Cannot get user ID.");

            var clientId = int.Parse(clientIdClaim);

            sc.ValidateClosure(complaintId, clientId, resolved);

            return Ok("Validation of the closure completed.");
        }


        [HttpGet("client/filter")]
        [Authorize(Roles = "Client")]
        public IActionResult FilterClientComplaints([FromQuery] ComplaintFilterRequest request)
        {
            var clientIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(clientIdClaim))
                return Unauthorized("Cannot get user ID.");

            var clientId = int.Parse(clientIdClaim);

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


        [HttpDelete("{complaintId}/files")]
        public IActionResult DeleteFile(int complaintId, [FromQuery] string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("File path is required.");

            var deleted = scf.RemoveFile(filePath, complaintId);

            if (!deleted)
                return NotFound("File not found for this complaint.");

            return Ok("File deleted successfully.");
        }


        //Endpoints Admin 

        [HttpGet("assigned")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAssignedComplaints()
        {
            var adminIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(adminIdClaim))
                return Unauthorized("Cannot get user ID.");

            var adminId = int.Parse(adminIdClaim);

            var complaints = sc.GetComplaintsByAdmin(adminId);
            var complaintDtos = complaints.Select(c => new ComplaintDto
            {
                ComplaintId = c.ComplaintId,
                Title = c.Title,
                Description = c.Description,
                SubmissionDate = c.SubmissionDate,
                ProcessedDate = c.ProcessedDate,
                ComplaintState = c.ComplaintState,
                ComplaintStatus = c.ComplaintStatus,
                ComplaintType = c.ComplaintType,
                FeatureFK = c.FeatureFK,
                ClientFK = c.ClientFK
            });
            return Ok(complaintDtos);
        }

        [HttpGet("admin/{complaintId}/details")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetComplaintDetailsForAdmin(int complaintId)
        {
            var adminIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(adminIdClaim))
                return Unauthorized("Cannot get user ID.");

            var adminId = int.Parse(adminIdClaim);

            var complaint = sc.GetComplaintDetailsByAdmin(adminId, complaintId);

            if (complaint == null)
                return NotFound("Claim not found, or not assigned to this admin.");

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
            var adminIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(adminIdClaim))
                return Unauthorized("Cannot get user ID.");

            var adminId = int.Parse(adminIdClaim);

            sc.UpdateComplaintState(adminId, complaintId, request.NewState);

            return Ok("Claim state is successfuly updated.");
        }


        [HttpPost("rollback/{complaintId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult RollbackComplaint(int complaintId)
        {
            var adminIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(adminIdClaim))
                return Unauthorized("Cannot get user ID.");

            var adminId = int.Parse(adminIdClaim);

            sc.RollbackComplaintToAgent(adminId, complaintId);

            return Ok("Claim is rolledback to the agent.");
        }

        [HttpGet("admin/filter")]
        [Authorize(Roles ="Admin")]
        public IActionResult FilterAdminComplaints([FromQuery] ComplaintFilterRequest req)
        {

            var adminIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(adminIdClaim))
                return Unauthorized("Cannot get user ID.");

            var adminId = int.Parse(adminIdClaim);

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
            var agentIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(agentIdClaim))
                return Unauthorized("Cannot get user ID.");

            var agentId = int.Parse(agentIdClaim);

            var complaints = sc.GetAll();
            var complaintDtos = complaints.Select(c => new ComplaintDto
            {
                ComplaintId = c.ComplaintId,
                Title = c.Title,
                Description = c.Description,
                SubmissionDate = c.SubmissionDate,
                ProcessedDate = c.ProcessedDate,
                ComplaintState = c.ComplaintState,
                ComplaintStatus = c.ComplaintStatus,
                ComplaintType = c.ComplaintType,
                FeatureFK = c.FeatureFK,
                ClientFK = c.ClientFK
            });
            return Ok(complaintDtos);
        }

        [HttpPost("assign/{complaintId}/{adminId}")]
        [Authorize(Roles = "Agent")]
        public IActionResult AssignComplaint(int complaintId, int adminId)
        {
            var agentIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(agentIdClaim))
                return Unauthorized("Cannot get user ID.");

            var agentId = int.Parse(agentIdClaim);

            sacl.AssignComplaintToAdmin(complaintId, agentId, adminId);

            return Ok("Claim is assigned to an admin.");
        }

        [HttpGet("agent/filter")]
        [Authorize(Roles = "Agent")]
        public IActionResult FilterAgentComplaints([FromQuery] ComplaintFilterRequest req)
        {
            var agentIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(agentIdClaim))
                return Unauthorized("Cannot get user ID.");

            var agentId = int.Parse(agentIdClaim);

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


        [HttpGet("download")]
        public IActionResult DownloadFile([FromQuery] string path)
        {
            try
            {
                var bytes = scf.DownloadFile(path);
                var fileName = Path.GetFileName(path);
                return File(bytes, "application/octet-stream", fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }







    }

    //Les Classes DTO

    public class ComplaintDto
    {
        public int ComplaintId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public State ComplaintState { get; set; }
        public Status ComplaintStatus { get; set; }
        public ComplaintType ComplaintType { get; set; }
        public int FeatureFK { get; set; }
        public int ClientFK { get; set; }
    }


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
        public string? filePath { get; set; }
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
