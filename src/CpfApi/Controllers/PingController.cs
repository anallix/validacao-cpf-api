using Microsoft.AspNetCore.Mvc;

namespace CpfApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        // GET api/ping
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API está rodando! ✅");
        }
    }
}
