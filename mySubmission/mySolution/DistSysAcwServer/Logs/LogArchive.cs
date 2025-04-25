using System;
using System.ComponentModel.DataAnnotations;

namespace DistSysAcwServer.Models
{
    #region Task 13
    // LogArchive entity to store logs of deleted users
    public class LogArchive
    {
        [Key]
        public int LogID { get; set; }

        public string LogString { get; set; }

        public DateTime LogDateTime { get; set; }

        public string UserApiKey { get; set; }

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