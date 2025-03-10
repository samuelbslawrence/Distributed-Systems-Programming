using DistSysAcwServer.Middleware;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    [Route("api/[Controller]/[Action]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected Models.UserContext DbContext { get; }
        protected SharedError Error { get; }

        public BaseController(Models.UserContext dbcontext, SharedError error)
        {
            DbContext = dbcontext;
            Error = error;
        }
    }
}