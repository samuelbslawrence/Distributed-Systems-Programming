using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

#region Task 9
namespace DistSysAcwServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class ProtectedController : ControllerBase
    {
        // GET: api/Protected/Hello
        [HttpGet]
        public IActionResult Hello()
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Bad Request");
            }
            return Ok($"Hello {userName}");
        }

        // GET: api/Protected/SHA1?message=hello
        [HttpGet("SHA1")]
        public IActionResult ComputeSHA1([FromQuery] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Bad Request");
            }

            using (var sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(message));
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("X2")); // uppercase hex
                }
                return Ok(sb.ToString());
            }
        }

        // GET: api/Protected/SHA256?message=hello
        [HttpGet("SHA256")]
        public IActionResult ComputeSHA256([FromQuery] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Bad Request");
            }

            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return Ok(sb.ToString());
            }
        }
    }
}
#endregion