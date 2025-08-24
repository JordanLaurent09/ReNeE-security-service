using Microsoft.EntityFrameworkCore;
using security_service.Database.Entities;

namespace security_service.Database.Context
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<RefreshToken> refreshSessions {  get; set; }

        public ApplicationDbContext(DbContextOptions options): base(options) 
        {
            Database.EnsureCreated();
        }

    }
}
