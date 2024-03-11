using MarcasAutosAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MarcasAutosAPI;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarBrand>().ToTable("MarcasAutos");
        modelBuilder.Entity<CarBrand>().Property(p => p.Name).HasColumnName("Nombre");
        // NOTA: Model seeding tiene muchos caveats que no vale pena hacerlo parte de migracion (ver https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding#model-seed-data)
    }

    public DbSet<CarBrand> CarBrands { get; set; }
}
