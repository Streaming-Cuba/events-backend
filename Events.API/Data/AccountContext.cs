using Events.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Data
{
    public class AccountContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        
        public AccountContext(DbContextOptions<AccountContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<AccountRole>(entity => {                
                entity.HasOne(d => d.Account)
                      .WithMany(p => p.Roles)
                      .HasForeignKey(d => d.AccountId);
            });
        }
    }
}