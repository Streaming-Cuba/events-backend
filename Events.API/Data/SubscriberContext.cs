using Events.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Data
{
    public class SubscriberContext : DbContext
    {
        public DbSet<Subscriber> Subscribers { get; set; }
        
        public SubscriberContext(DbContextOptions<SubscriberContext> options) : base(options) { }
    }
}