using Microsoft.EntityFrameworkCore;

namespace DistSysAcwServer.Models
{
    public class UserContext : DbContext
    {
        // Represents the Users table in the database
        public DbSet<User> Users { get; set; }

        // Represents the Logs table for active log entries
        public DbSet<Log> Logs { get; set; }

        // Represents the LogArchives table for archived log entries
        public DbSet<LogArchive> LogArchives { get; set; }

        // Constructor accepting configuration options for the DbContext
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }
    }
}