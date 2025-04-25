using DistSysAcwServer.Middleware;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtherController : BaseController
    {
        private readonly Models.UserContext _dbContext;

        /// <summary>
        /// Constructs an Other controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="dbcontext">DbContext set as a service in Startup.cs and dependency injected</param>
        /// <param name="error">Shared error object for consistent error handling</param>
        public OtherController(Models.UserContext dbcontext, SharedError error) : base(dbcontext, error)
        {
            _dbContext = dbcontext;
        }

        // Get api/other/clear
        [HttpGet("clear")]
        public IActionResult Clear()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            return Ok("Success, all data cleared.");
        }
    }
}