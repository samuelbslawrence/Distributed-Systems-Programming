using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace DistSysAcwServer.Middleware
{
    public class ActionErrorHandlingFilter : IAlwaysRunResultFilter, IExceptionFilter
    {
        private readonly SharedError _error;

        public ActionErrorHandlingFilter(SharedError error)
        {
            _error = error;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result != null && _error.Message != null)
            {
                context.HttpContext.Response.ContentType = "text/plain";
                context.HttpContext.Response.StatusCode = _error.StatusCode ?? StatusCodes.Status500InternalServerError;
                context.HttpContext.Response.WriteAsync(_error.Message);
            }
            else if (context.HttpContext.Response.StatusCode >= 400)
            {
                context.HttpContext.Response.ContentType = "text/plain";
                context.HttpContext.Response.WriteAsync("An unexpected error occurred");
            }
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            OnResultExecuting(context);
            await next();
        }

        public void OnResultExecuted(ResultExecutedContext context) { }

        public void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.ContentType = "text/plain";
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.HttpContext.Response.WriteAsync($"An exception occurred in {context.ActionDescriptor.DisplayName}: ({context.Exception.Message})");
        }
    }
}