﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DistSysAcwServer.Middleware;
using DistSysAcwServer.Models;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;


namespace DistSysAcwServer.Auth
{
    /// <summary>
    /// Authenticates clients by API Key
    /// </summary>
    public class CustomAuthenticationHandlerMiddleware
        : AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationHandler
    {
        private Models.UserContext DbContext { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private SharedError Error { get; set; }

        public CustomAuthenticationHandlerMiddleware(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            Models.UserContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            SharedError error)
            : base(options, logger, encoder)
        {
            DbContext = dbContext;
            HttpContextAccessor = httpContextAccessor;
            Error = error;
        }

        /// <summary>
        /// Authenticates the client by API Key
        /// </summary>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then create the correct Claims, add these to a ClaimsIdentity, create a ClaimsPrincipal from the identity 
            //        Then use the Principal to generate a new AuthenticationTicket to return a Success AuthenticateResult
            #endregion

            return Task.FromResult(AuthenticateResult.Fail(new AuthenticationFailureException("HandleAuthenticateAsync is not yet fully implemented"))); // Placeholder
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Error.StatusCode = StatusCodes.Status501NotImplemented;
            Error.Message = "Task 5 incomplete";
            return Task.CompletedTask;
        }
    }
}