using Microsoft.EntityFrameworkCore;

namespace DistSysAcwServer.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<LogArchive> LogArchives { get; set; }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }
    }
}