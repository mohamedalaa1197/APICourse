using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Pluralsight.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class OperationsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public OperationsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpOptions]
        public IActionResult reload()
        {
            try
            {
                var root=(IConfigurationRoot)_config;
                root.Reload();
                return Ok();
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,"Database Failure");                
            }
        }
    }
}