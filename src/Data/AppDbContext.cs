using Microsoft.EntityFrameworkCore;
using SiteSB.Models;

namespace SiteSB.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ClientCategory> ClientCategories { get; set; }
        public DbSet<Depositor> Depositors { get; set; }
        public DbSet<DepositType> DepositTypes { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<DepositAccrual> DepositAccruals { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурации отношений
            modelBuilder.Entity<Depositor>()
                .HasIndex(d => d.INN)
                .IsUnique();

            modelBuilder.Entity<Deposit>()
                .HasIndex(d => d.ContractNumber)
                .IsUnique();
        }
    }
}
