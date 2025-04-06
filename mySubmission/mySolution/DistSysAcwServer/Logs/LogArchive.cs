using System;
using System.ComponentModel.DataAnnotations;

namespace DistSysAcwServer.Models
{
    #region Task 13
    // LogArchive entity to store logs of deleted users
    public class LogArchive
    {
        [Key]
        public int LogID { get; set; } // Primary key

        public string LogString { get; set; } // Log description

        public DateTime LogDateTime { get; set; } // Timestamp of the log

        public string UserApiKey { get; set; } // API key of the user to whom this log belonged

        // Constructor that sets the log message, associated user API key and current time
        public LogArchive(string logMessage, string userApiKey)
        {
            LogString = logMessage;
            LogDateTime = DateTime.Now;
            UserApiKey = userApiKey;
        }

        // Parameterless constructor for EF Core
        public LogArchive() { }
    }
    #endregion
}