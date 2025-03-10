using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DistSysAcwServer.Middleware;
using DistSysAcwServer.Shared;

namespace DistSysAcwServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TalkbackController : BaseController
    {
        public TalkbackController(Models.UserContext dbcontext, SharedError error) : base(dbcontext, error) { }

        // Task 1: Hello endpoint
        [HttpGet]
        public IActionResult Hello()
        {
            return Ok("Hello, world!");
        }

        // Task 1: Sort endpoint
        [HttpGet]
        public IActionResult Sort([FromQuery] int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
            {
                Error.StatusCode = 400;
                Error.Message = "Bad Request: No numbers provided.";
                return BadRequest(Error.Message);
            }

            var sortedNumbers = numbers.OrderBy(n => n).ToArray();
            return Ok(sortedNumbers);
        }
    }
}