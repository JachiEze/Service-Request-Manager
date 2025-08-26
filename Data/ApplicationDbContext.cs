using Microsoft.EntityFrameworkCore;
using ServiceRequestForm.Models;

namespace ServiceRequestForm.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>().HasData(
                new UserAccount { Id = 1, Username = "divineezenwamadu@gmail.com", Password = "validator123", Role = "Validator" },
                new UserAccount { Id = 2, Username = "memesrus5000@gmail.com", Password = "approver123", Role = "Approver" }
            );
        }
    }
}

