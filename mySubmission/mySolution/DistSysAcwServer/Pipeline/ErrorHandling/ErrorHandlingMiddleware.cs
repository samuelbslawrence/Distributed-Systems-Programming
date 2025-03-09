﻿using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using DistSysAcwServer.Middleware;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.Serialization;
using DistSysAcwServer.Shared;

namespace DistSysAcwServer.Pipeline
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private SharedError Error { get; set; }

        public ErrorHandlingMiddleware(SharedError error)
        {
            Error = error;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);

            // Check if an error was set in middleware (this ensures we catch all errors set in the middleware, not only errors from actions)
            if (Error.Message != null && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                if (Error.StatusCode != null)
                {
                    context.Response.StatusCode = (int)Error.StatusCode;
                }
                await context.Response.WriteAsync(Error.Message);
            }
        }
    }
}
