using ClosedXML;
using DirectCompanies.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace DirectCompanies.Services
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<decimal>, decimal>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<MedicalContractClass> MedicalContractClasss { get; set; }
        public DbSet<BeneficiaryType> BeneficiaryTypes { get; set; }
        public DbSet<OutBoxEvent> OutBoxEvents { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne<MedicalContractClass>(s => s.MedicalContractClass)
                .WithMany(g => g.Employees)
                .HasForeignKey(s => s.MedicalContractClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasOne<BeneficiaryType>(s => s.BeneficiaryType)
                .WithMany(g => g.Employees)
                .HasForeignKey(s => s.BeneficiaryTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
           .HasOne(e => e.ApplicationUser) 
           .WithMany(u => u.Employees)     
           .HasForeignKey(e => e.MedicalCustomerId) 
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OutBoxEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();


            });
          

        }


    }
}
