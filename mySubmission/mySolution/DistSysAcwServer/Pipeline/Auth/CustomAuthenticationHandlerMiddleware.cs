using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DistSysAcwServer.Models;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DistSysAcwServer.Auth
{
    public class CustomAuthenticationHandlerMiddleware : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SharedError _error;

        public CustomAuthenticationHandlerMiddleware(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            UserContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            SharedError error)
            : base(options, logger, encoder)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _error = error;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("ApiKey", out var apiKey))
            {
                return AuthenticateResult.Fail("Missing API Key");
            }

            var user = await _dbContext.Users.FindAsync(Guid.Parse(apiKey));
            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            _error.StatusCode = StatusCodes.Status401Unauthorized;
            _error.Message = "Unauthorized: API Key is required";
            return Task.CompletedTask;
        }
    }
}