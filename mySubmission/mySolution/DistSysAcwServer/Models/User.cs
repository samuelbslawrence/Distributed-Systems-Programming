using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DistSysAcwServer.Models
{
    /// <summary>
    /// Represents an application user with authentication key, role, and related logs
    /// </summary>
    public class User
    {
        #region Task 2
        // Parameterless constructor required by EF Core
        public User() { }

        // Primary key: unique API key (GUID stored as string)
        [Key]
        public string ApiKey { get; set; }

        // The user's display or login name
        public string UserName { get; set; }

        // Role for authorization (e.g., "Admin" or "User")
        public string Role { get; set; }
        #endregion

        #region Task 13
        // Collection of Log entries related to this user
        public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
        #endregion
    }
}