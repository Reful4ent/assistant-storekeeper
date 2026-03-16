using Microsoft.EntityFrameworkCore;

namespace assistant_storekeeper_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Позже: public DbSet<YourEntity> YourEntities { get; set; }
    }
}