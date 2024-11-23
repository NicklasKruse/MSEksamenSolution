using Administration.Domain.Entities;
using Administration.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Administration.ContextLibrary
{
    public class AdministrationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Animal> Animals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Animal>().HasKey(a => a.Id);
            modelBuilder.Entity<Animal>().Property(a => a.SpeciesId)
                .HasConversion(speciesId => speciesId.Value, speciesId => SpeciesId.Create(speciesId));
            modelBuilder.Entity<Animal>().HasOne(a => a.Weight);
        }
    }
}
