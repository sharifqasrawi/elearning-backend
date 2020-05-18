using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;

            return Ok( remoteIpAddress.ToString());
        }


    }
}