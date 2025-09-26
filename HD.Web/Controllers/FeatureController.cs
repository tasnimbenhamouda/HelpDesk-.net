using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using HD.ApplicationCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HD.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        IServiceFeature sf;

        public FeatureController(IServiceFeature sf)
        {   
            this.sf = sf;
        }

        [HttpGet("{featureId}/name")]
        public IActionResult GetFeatureName(int featureId)
        {
            var name = sf.GetFeatureNameById(featureId);

            if (name == null)
                return NotFound("Feature not found");

            return Ok(name);
        }

        [HttpGet("all")]
        public IActionResult GetFeatures()
        {
            var features = sf.GetAll()
                     .Select(f => new FeatureDto
                     {
                         FeatureId = f.FeatureId,
                         Name = f.Name,
                         Description = f.Description
                     })
                     .ToList();


            return Ok(features);
        }
    }

    public class FeatureDto
    {
        public int FeatureId { get; set; }
        public string Name { get; set; }
        public string Description
        {
            get; set;
        }
    }
}
