using Microsoft.EntityFrameworkCore;

namespace DistSysAcwServer.Models
{
    public class UserContext : DbContext
    {
        public UserContext() : base() { }

        public required DbSet<User> Users { get; set; }

        #region Task 13
        // DbSet for current user logs
        public DbSet<Log> Logs { get; set; }
        // DbSet for archived logs (logs of deleted users)
        public DbSet<LogArchive> LogArchives { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DistSysAcw;");
        }
    }
}