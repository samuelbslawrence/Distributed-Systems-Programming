using DistSysAcwServer.Controllers;
using DistSysAcwServer.Models;
using DistSysAcwServer.Repositories;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysAcwServer.Controllers
{
    #region Task 4
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserRepository _userRepository;

        public UserController(
            UserContext dbcontext,
            SharedError error,
            UserRepository userRepository) : base(dbcontext, error)
        {
            _userRepository = userRepository;
        }

        [HttpGet("new")]
        public async Task<IActionResult> CheckUserExists([FromQuery] string? username)
        {
            // Validate that 'username' parameter is not null or whitespace
            if (string.IsNullOrWhiteSpace(username))
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }

            // Check database for matching username
            bool userExists = await DbContext.Users.AnyAsync(u => u.UserName == username);

            // Return result: true if exists, false otherwise
            if (userExists)
            {
                return Ok("True - User Does Exist! Did you mean to do a POST to create a new user?");
            }
            else
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] string? username)
        {
            // Ensure request body contains a valid username
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }

            // Prevent duplicate usernames
            bool userExists = await DbContext.Users.AnyAsync(u => u.UserName == username);
            if (userExists)
            {
                return StatusCode(403, "Oops. This username is already in use. Please try again with a new username.");
            }

            // Assign 'Admin' role to the very first user, otherwise 'User'
            bool isFirstUser = !await DbContext.Users.AnyAsync();
            string role = isFirstUser ? "Admin" : "User";

            // Instantiate new User with a unique API key
            var newUser = new User
            {
                ApiKey = Guid.NewGuid().ToString(),
                UserName = username,
                Role = role
            };

            // Add and persist the new user
            DbContext.Users.Add(newUser);
            await DbContext.SaveChangesAsync();

            // Respond with the new API key
            return Ok(newUser.ApiKey);
        }

        #endregion

        #region Task 7 & 13
        [HttpDelete("removeuser")]
        public async Task<IActionResult> RemoveUser([FromQuery] string username)
        {
            // Check if the ApiKey header exists
            if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
            {
                return Ok(false);
            }
            string apiKey = apiKeyValues.First();

            // Task 13: Log the request for RemoveUser endpoint
            await _userRepository.AddLogAsync(apiKey, "User requested /api/user/removeuser");

            // Get the user by ApiKey using the repository
            var user = await _userRepository.GetUserByApiKeyAsync(apiKey);
            if (user == null)
            {
                return Ok(false);
            }

            // Check if the provided username matches the user's username (case-insensitive)
            if (!string.Equals(user.UserName, username, System.StringComparison.OrdinalIgnoreCase))
            {
                return Ok(false);
            }

            // Attempt to delete the user
            bool deleted = await _userRepository.DeleteUserAsync(apiKey);
            return Ok(deleted);
        }
        #endregion

        #region Task 8 & 13
        public class ChangeRoleRequest
        {
            public string Username { get; set; }
            public string Role { get; set; }
        }

        [HttpPost("changerole")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleRequest request)
        {
            try
            {
                // Check if the ApiKey header exists
                if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
                {
                    return BadRequest("NOT DONE: An error occured");
                }
                string apiKey = apiKeyValues.First();

                // Task 13: Log the request for ChangeRole endpoint
                await _userRepository.AddLogAsync(apiKey, "User requested /api/user/changerole");

                // Get the user by ApiKey using the repository and verify admin privileges
                var adminUser = await _userRepository.GetUserByApiKeyAsync(apiKey);
                if (adminUser == null || !string.Equals(adminUser.Role, "Admin", System.StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("NOT DONE: An error occured");
                }

                // Validate the role provided in the request. Only allow "User" or "Admin"
                if (!string.Equals(request.Role, "User", System.StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(request.Role, "Admin", System.StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("NOT DONE: Role does not exist");
                }

                // Check if the username exists in the database
                var targetUser = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
                if (targetUser == null)
                {
                    return BadRequest("NOT DONE: Username does not exist");
                }

                // Update the user's role
                targetUser.Role = request.Role;
                await DbContext.SaveChangesAsync();

                return Ok("DONE");
            }
            catch (System.Exception)
            {
                return BadRequest("NOT DONE: An error occured");
            }
        }
        #endregion
    }
}