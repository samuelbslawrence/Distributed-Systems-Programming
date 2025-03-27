using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DistSysAcwServer.Middleware;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace DistSysAcwServer.Auth
{
    /// <summary>
    /// Authorises clients by role
    /// </summary>
    public class CustomAuthorizationHandlerMiddleware : AuthorizationHandler<RolesAuthorizationRequirement>, IAuthorizationHandler
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private SharedError Error { get; set; }

        public CustomAuthorizationHandlerMiddleware(IHttpContextAccessor httpContextAccessor, SharedError error)
        {
            HttpContextAccessor = httpContextAccessor;
            Error = error;
        }

        /// <summary>
        /// Handles success or failure of the requirement for the user to be in a specific role
        /// </summary>
        /// <param name="context">Information used to decide on authorisation</param>
        /// <param name="requirement">Authorisation requirements</param>

        #region Task 6
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            // Fail if no user is authenticated
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // Check if the action requires Admin role
            bool isAdminRequired = requirement.AllowedRoles.Contains("Admin");

            // Get the user's current role
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (isAdminRequired && userRole != "Admin")
            {
                // Set error for Forbidden access
                Error.StatusCode = StatusCodes.Status403Forbidden;
                Error.Message = "Forbidden. Admin access only.";

                context.Fail();
                return Task.CompletedTask;
            }

            // If we reach here and the role matches, succeed the requirement
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        #endregion
    }
}