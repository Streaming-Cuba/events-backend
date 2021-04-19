using Events.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Data
{
    public class AccountContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        public AccountContext(DbContextOptions<AccountContext> options) : base(options) { }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     // optionsBuilder.UseNpgsql();
        // }
    }
}