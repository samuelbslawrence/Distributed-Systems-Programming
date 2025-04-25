using System;
using System.ComponentModel.DataAnnotations;

namespace DistSysAcwServer.Models
{
    #region Task 13
    // Log entity representing a user action log
    public class Log
    {
        [Key]
        public int LogID { get; set; }

        public string LogString { get; set; }

        public DateTime LogDateTime { get; set; }

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