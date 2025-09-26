using HD.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HD.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        IServiceAdmin sa;

        public AdminController(IServiceAdmin sa)
        {
            this.sa = sa;
        }

        [HttpGet("{adminId}/name")]
        public IActionResult GetAdmintName(int adminId)
        {
            try
            {
                var name = sa.GetAdminName(adminId);
                return Ok(name);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
