using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace DistSysAcwServer.Auth
{
    public class CustomAuthorizationHandlerMiddleware : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SharedError _error;

        public CustomAuthorizationHandlerMiddleware(IHttpContextAccessor httpContextAccessor, SharedError error)
        {
            _httpContextAccessor = httpContextAccessor;
            _error = error;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            var user = context.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (requirement.AllowedRoles.Any(role => user.IsInRole(role)))
            {
                context.Succeed(requirement);
            }
            else
            {
                _error.StatusCode = StatusCodes.Status403Forbidden;
                _error.Message = "Forbidden: You do not have the required role.";
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}