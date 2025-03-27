using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DistSysAcwServer.Middleware;
using DistSysAcwServer.Models;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
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
        /// 

        #region Task 5
        // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
        //        Then create the correct Claims, add these to a ClaimsIdentity, create a ClaimsPrincipal from the identity 
        //        Then use the Principal to generate a new AuthenticationTicket to return a Success AuthenticateResult

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if ApiKey header exists
            if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                // No ApiKey header present
                return AuthenticateResult.NoResult();
            }

            // Get the first ApiKey value
            string apiKey = apiKeyValues.First();

            // Find user with the given API Key
            var user = await DbContext.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);

            // If user not found, authentication fails
            if (user == null)
            {
                return AuthenticateResult.Fail("Unauthorized. Check ApiKey in Header is correct.");
            }

            // Create claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create ClaimsIdentity
            var identity = new ClaimsIdentity(claims, "ApiKey");

            // Create ClaimsPrincipal
            var principal = new ClaimsPrincipal(identity);

            // Create Authentication Ticket
            var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

            // Return successful authentication
            return AuthenticateResult.Success(ticket);
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Error.StatusCode = StatusCodes.Status401Unauthorized;
            Error.Message = "Unauthorized. Check ApiKey in Header is correct.";
            return Task.CompletedTask;
        }
        #endregion
    }
}