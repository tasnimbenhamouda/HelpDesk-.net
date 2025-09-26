using HD.ApplicationCore.Interfaces;
using HD.ApplicationCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HD.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        IServiceClient sc;

        public ClientController(IServiceClient sc)
        {
            
            this.sc = sc;
        }

        [HttpGet("{clientId}/name")]
        public IActionResult GetClientName(int clientId)
        {
            var name = sc.GetClientName(clientId);

            if (name == null)
                return NotFound("Client not found");

            return Ok(name);
        }
    }
}
