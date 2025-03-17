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


        #region TASK1
        //    TODO: add api/talkback/hello response
        [HttpGet("api/talkback/hello")]
        public IActionResult Hello()
        {
            return Ok("Hello World");
        }
        #endregion

        #region TASK1
        //    TODO:
        //       add a parameter to get integers from the URI query
        //       sort the integers into ascending order
        //       send the integers back as the api/talkback/sort response
        //       conform to the error handling requirements in the spec

        [HttpGet("api/talkback/sort")]
        public IActionResult Sort([FromQuery] List<string> integers)
        {
            if (integers == null || integers.Count == 0)
            {
                return Ok(new List<int>());
            }

            List<int> sortedIntegers = new List<int>();
            foreach (var str in integers)
            {
                if (!int.TryParse(str, out int num))
                {
                    return BadRequest("Invalid input: all values must be integers.");
                }
                sortedIntegers.Add(num);
            }

            sortedIntegers.Sort();
            return Ok(sortedIntegers);
        }
        #endregion
    }
}