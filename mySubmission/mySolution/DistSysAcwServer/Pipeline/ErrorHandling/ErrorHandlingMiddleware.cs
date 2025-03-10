using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using DistSysAcwServer.Shared;

namespace DistSysAcwServer.Pipeline
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly SharedError _error;

        public ErrorHandlingMiddleware(SharedError error)
        {
            _error = error;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);

            if (!context.Response.HasStarted && _error.Message != null)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = _error.StatusCode ?? StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(_error.Message);
            }
        }
    }
}