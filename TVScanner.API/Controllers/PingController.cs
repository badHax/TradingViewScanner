using Microsoft.AspNetCore.Mvc;

namespace TVScanner.API.Controllers
{
    [Produces("text/plain")]
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Pong");
        }
    }
}
