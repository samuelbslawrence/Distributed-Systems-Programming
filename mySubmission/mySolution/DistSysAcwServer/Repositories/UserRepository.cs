using DistSysAcwServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysAcwServer.Repositories
{
    #region Task 3
    public class UserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a new user with a generated ApiKey
        /// </summary>
        /// <param name="userName">Username for the new user</param>
        /// <returns>The newly created User object</returns>
        public async Task<User> CreateUserAsync(string userName)
        {
            // Generate a new unique ApiKey
            string apiKey = Guid.NewGuid().ToString();

            // Create new user object
            var newUser = new User
            {
                ApiKey = apiKey,
                UserName = userName,
                Role = "User" // Default role
            };

            // Add to context and save
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        /// <summary>
        /// Check if a user with the given ApiKey exists
        /// </summary>
        /// <param name="apiKey">ApiKey to check</param>
        /// <returns>True if user exists, False otherwise</returns>
        public async Task<bool> UserExistsAsync(string apiKey)
        {
            return await _context.Users.AnyAsync(u => u.ApiKey == apiKey);
        }

        /// <summary>
        /// Check if a user with the given ApiKey and Username exists
        /// </summary>
        /// <param name="apiKey">ApiKey to check</param>
        /// <param name="userName">Username to check</param>
        /// <returns>True if user exists, False otherwise</returns>
        public async Task<bool> UserExistsAsync(string apiKey, string userName)
        {
            return await _context.Users.AnyAsync(u => u.ApiKey == apiKey && u.UserName == userName);
        }

        /// <summary>
        /// Get a user by their ApiKey
        /// </summary>
        /// <param name="apiKey">ApiKey of the user</param>
        /// <returns>User object if found, null otherwise</returns>
        public async Task<User?> GetUserByApiKeyAsync(string apiKey)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
        }

        /// <summary>
        /// Delete a user by their ApiKey
        /// </summary>
        /// <param name="apiKey">ApiKey of the user to delete</param>
        /// <returns>True if user was deleted, False if user not found</returns>
        public async Task<bool> DeleteUserAsync(string apiKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Update a user's role
        /// Uses optimistic concurrency to handle simultaneous updates
        /// </summary>
        /// <param name="apiKey">ApiKey of the user</param>
        /// <param name="newRole">New role to assign</param>
        /// <returns>True if update successful, False if user not found</returns>
        public async Task<bool> UpdateUserRoleAsync(string apiKey, string newRole)
        {
            try
            {
                // Use Update method to handle potential concurrent modifications
                var user = await _context.Users
                    .Where(u => u.ApiKey == apiKey)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Role, newRole)
                    );

                return user > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Log the concurrency exception if needed
                return false;
            }
        }
    }
    #endregion
}