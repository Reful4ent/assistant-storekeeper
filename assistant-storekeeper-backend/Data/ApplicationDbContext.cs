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
            ConfigureCompanyWarehouse(modelBuilder);
            ConfigureNomenclature(modelBuilder);
            ConfigureMovement(modelBuilder);
            ConfigureMovementNomenclature(modelBuilder);
            ConfigureCompanyWarehouseNomenclature(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureCompanyWarehouse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyWarehouse>()
                .Property(c => c.Name)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<CompanyWarehouse>()
                .HasIndex(c => c.Name)
                .IsUnique();
        }

        private void ConfigureNomenclature(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Nomenclature>()
                .Property(c => c.Name)
                .HasMaxLength(255)
                .IsRequired();
            modelBuilder.Entity<Nomenclature>()
                .HasIndex(c => c.Name)
                .IsUnique();
        }

        private void ConfigureMovement(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movement>()
                .Property(c => c.Date)
                .IsRequired();
            modelBuilder.Entity<Movement>()
                .Property(c => c.Status)
                .IsRequired();
        }

        private void ConfigureMovementNomenclature(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovementNomenclature>()
                .Property(c => c.MovementId)
                .IsRequired();
            modelBuilder.Entity<MovementNomenclature>()
                .Property(c => c.NomenclatureId)
                .IsRequired();
            modelBuilder.Entity<MovementNomenclature>()
                .Property(c => c.Quantity)
                .IsRequired();
            modelBuilder.Entity<MovementNomenclature>()
                .HasIndex(c => new { c.MovementId, c.NomenclatureId })
                .IsUnique();
        }

        private void ConfigureCompanyWarehouseNomenclature(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyWarehouseNomenclature>()
                .Property(c => c.CompanyWarehouseId)
                .IsRequired();
            modelBuilder.Entity<CompanyWarehouseNomenclature>()
                .Property(c => c.NomenclatureId)
                .IsRequired();
            modelBuilder.Entity<CompanyWarehouseNomenclature>()
                .Property(c => c.Quantity)
                .IsRequired();
        }
    }
}