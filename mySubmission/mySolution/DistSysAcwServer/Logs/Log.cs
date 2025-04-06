using System;
using System.ComponentModel.DataAnnotations;

namespace DistSysAcwServer.Models
{
    #region Task 13
    // Log entity representing a user action log
    public class Log
    {
        [Key]
        public int LogID { get; set; } // Primary key

        public string LogString { get; set; } // Description of the log

        public DateTime LogDateTime { get; set; } // Timestamp of the log

        // Constructor that sets the log message and current time
        public Log(string logMessage)
        {
            LogString = logMessage;
            LogDateTime = DateTime.Now;
        }

        // Parameterless constructor for EF Core
        public Log() { }
    }
    #endregion
}