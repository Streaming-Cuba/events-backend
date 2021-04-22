using Events.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Data
{
    public class AccountContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        
        public AccountContext(DbContextOptions<AccountContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Account>()
                .HasIndex(x => x.Email)
                .IsUnique();   

            modelBuilder.Entity<Permission>()
                .HasIndex(x => x.Name)
                .IsUnique();        
                 
            modelBuilder.Entity<Role>()
                .HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}