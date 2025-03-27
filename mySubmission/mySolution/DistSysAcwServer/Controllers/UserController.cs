using DistSysAcwServer.Controllers;
using DistSysAcwServer.Models;
using DistSysAcwServer.Repositories;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DistSysAcwServer.Controllers
{
    #region Task 4
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

        [HttpGet("api/user/new")]
        public async Task<IActionResult> CheckUserExists([FromQuery] string? username)
        {
            // If no username is provided
            if (string.IsNullOrWhiteSpace(username))
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }

            // Check if user exists by username
            bool userExists = await DbContext.Users.AnyAsync(u => u.UserName == username);

            if (userExists)
            {
                return Ok("True - User Does Exist! Did you mean to do a POST to create a new user?");
            }
            else
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }
        }

        [HttpPost("api/user/new")]
        public async Task<IActionResult> CreateUser([FromBody] string? username)
        {
            // Check if username is provided
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }

            // Check if username is already taken
            bool userExists = await DbContext.Users.AnyAsync(u => u.UserName == username);
            if (userExists)
            {
                return StatusCode(403, "Oops. This username is already in use. Please try again with a new username.");
            }

            // Determine if this is the first user (should be admin)
            bool isFirstUser = !await DbContext.Users.AnyAsync();
            string role = isFirstUser ? "Admin" : "User";

            // Create new user
            var newUser = new User
            {
                ApiKey = Guid.NewGuid().ToString(),
                UserName = username,
                Role = role
            };

            // Add user to database
            DbContext.Users.Add(newUser);
            await DbContext.SaveChangesAsync();

            // Return API Key
            return Ok(newUser.ApiKey);
        }

    #endregion

    #region Task 7
    [HttpDelete("api/user/removeuser")]
    public async Task<IActionResult> RemoveUser([FromQuery] string username)
    {
        // Check if the ApiKey header exists
        if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
        {
            return Ok(false);
        }
        string apiKey = apiKeyValues.First();

        // Get the user by ApiKey using the repository
        var user = await _userRepository.GetUserByApiKeyAsync(apiKey);
        if (user == null)
        {
            return Ok(false);
        }

        // Check if the provided username matches the user's username (case-insensitive)
        if (!string.Equals(user.UserName, username, StringComparison.OrdinalIgnoreCase))
        {
            return Ok(false);
        }

        // Attempt to delete the user
        bool deleted = await _userRepository.DeleteUserAsync(apiKey);
        return Ok(deleted);
    }
    #endregion

    #region Task 8
    public class ChangeRoleRequest
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }

    [HttpPost("api/user/changerole")]
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

            // Get the user by ApiKey using the repository and verify admin privileges
            var adminUser = await _userRepository.GetUserByApiKeyAsync(apiKey);
            if (adminUser == null || !string.Equals(adminUser.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("NOT DONE: An error occured");
            }

            // Validate the role provided in the request. Only allow "User" or "Admin"
            if (!string.Equals(request.Role, "User", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase))
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
        catch (Exception)
        {
            return BadRequest("NOT DONE: An error occured");
        }
        }
    #endregion
    }
}