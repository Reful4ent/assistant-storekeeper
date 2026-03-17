using Microsoft.EntityFrameworkCore;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<CompanyWarehouse> CompanyWarehouses { get; set; }
        public DbSet<CompanyWarehouseNomenclature> CompanyWarehouseNomenclatures { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<MovementNomenclature> MovementNomenclatures { get; set; }
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.HasPostgresEnum<MovementStatus>("public", "movement_status");
            modelBuilder.Entity<MovementNomenclature>()
                .HasIndex(mn => new { mn.MovementId, mn.NomenclatureId })
                .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}