using Microsoft.EntityFrameworkCore;
using Veterinarian.Domain;
using Veterinarian.Domain.ValueObjects;

namespace Veterinarian.ContextLibrary
{
    public class VeterinarianDbContext(DbContextOptions options) : DbContext(options)
    {
        DbSet<Appointment> Appointments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<Appointment>().HasKey(a => a.Id);
            modelbuilder.Entity<Appointment>().Property(a => a.CaseId)
                .HasConversion(caseId => caseId.Value, caseId => CaseId.Create(caseId));
        }
    }
}
