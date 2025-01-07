using BankAccountApp.Classes;
using BankAccountApp.Interface;
using Microsoft.EntityFrameworkCore;

namespace BankAccountApp.Classes
{
    public class BankDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=BB8\\SQLEXPRESS;Database=BankDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>()
                .Property(b => b.Balance)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.BankAccount)
                .WithMany(b => b.Transactions)
                .HasForeignKey(t => t.BankAccountId);
        }
    }
}