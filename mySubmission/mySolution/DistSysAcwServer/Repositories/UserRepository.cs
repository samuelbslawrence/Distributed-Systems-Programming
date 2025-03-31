using DistSysAcwServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysAcwServer.Repositories
{
    public class UserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        #region Task 3
        // Re-written for task 13
        // Creates a new user with a generated ApiKey
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

        // Checks if a user with the given ApiKey exists
        public async Task<bool> UserExistsAsync(string apiKey)
        {
            return await _context.Users.AnyAsync(u => u.ApiKey == apiKey);
        }

        // Checks if a user with the given ApiKey and Username exists
        public async Task<bool> UserExistsAsync(string apiKey, string userName)
        {
            return await _context.Users.AnyAsync(u => u.ApiKey == apiKey && u.UserName == userName);
        }

        // Retrieves a user by their ApiKey
        public async Task<User?> GetUserByApiKeyAsync(string apiKey)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
        }

        // Deletes a user by their ApiKey
        public async Task<bool> DeleteUserAsync(string apiKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Updates a user's role using optimistic concurrency
        public async Task<bool> UpdateUserRoleAsync(string apiKey, string newRole)
        {
            try
            {
                var affectedRows = await _context.Users
                    .Where(u => u.ApiKey == apiKey)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Role, newRole)
                    );

                return affectedRows > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Log the concurrency exception if needed
                return false;
            }
        }
        #endregion

        #region Task 13
        // Adds a log entry for the user identified by the given API key
        public async Task AddLogAsync(string apiKey, string logMessage)
        {
            // Include logs to ensure we have access to the collection
            var user = await _context.Users.Include(u => u.Logs).FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user != null)
            {
                // Add a new log entry with the message and current DateTime
                user.Logs.Add(new Log(logMessage));
                await _context.SaveChangesAsync();
            }
        }

        // Deletes a user and archives their logs before deletion
        public async Task<bool> DeleteUserWithLoggingAsync(string apiKey)
        {
            // Include logs in the query
            var user = await _context.Users.Include(u => u.Logs).FirstOrDefaultAsync(u => u.ApiKey == apiKey);

            if (user == null)
                return false;

            // Archive the user's logs so they remain after deletion
            if (user.Logs != null && user.Logs.Any())
            {
                foreach (var log in user.Logs)
                {
                    var archiveLog = new LogArchive(log.LogString, user.ApiKey);
                    _context.LogArchives.Add(archiveLog);
                }
            }

            // Remove the user from the database
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion
    }
}
