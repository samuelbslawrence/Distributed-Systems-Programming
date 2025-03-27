using System.Collections.Generic;
using DistSysAcwServer.Middleware;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    public class TalkbackController : BaseController
    {


        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkbackController(Models.UserContext dbcontext, SharedError error) : base(dbcontext, error) { }


        #region Task 1
        // GET api/talkback/hello
        [HttpGet]
        public IActionResult Hello()
        {
            // New implementation: return a friendly greeting.
            return Ok("Hello World");
        }

        // GET api/talkback/sort?integers=5&integers=3&integers=9
        [HttpGet]
        public IActionResult Sort([FromQuery] List<string> integers)
        {
            // If no integers provided, return an empty list.
            if (integers == null || integers.Count == 0)
            {
                return Ok(new List<int>());
            }

            List<int> sortedIntegers = new List<int>();

            // Try parsing each query string value into an integer.
            foreach (var str in integers)
            {
                if (!int.TryParse(str, out int num))
                {
                    // Set error details as per the error handling requirements.
                    Error.StatusCode = 400;
                    Error.Message = "Invalid input: all values must be integers.";
                    return BadRequest(Error.Message);
                }
                sortedIntegers.Add(num);
            }

            sortedIntegers.Sort();
            return Ok(sortedIntegers);
        }
        #endregion
    }
}