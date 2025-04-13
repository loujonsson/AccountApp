using AccountApp.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AccountApp.Data
{
    public class AccountAppDbContext : DbContext
    {
        public AccountAppDbContext(DbContextOptions<AccountAppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CustomerId);

            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, FirstName = "Anna", LastName = "Johansson", PhoneNumber = "0712345672" },
                new Customer { CustomerId = 2, FirstName = "Test", LastName = "Testsson", PhoneNumber = "0723123453" },
                new Customer { CustomerId = 3, FirstName = "Anton", LastName = "Eriksson", PhoneNumber = "0732442942"}
            );

            modelBuilder.Entity<Account>().HasData(
                new Account { CustomerId = 1, AccountId = 1, CreationTimestamp = new DateTime(2025, 04, 13), UpdatedTimestamp = null, Status = Models.Enums.AccountStatus.Active, Balance = 1000 });
        }
    }
}
