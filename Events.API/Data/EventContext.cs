using Events.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Data
{
    public class EventContext : DbContext
    {
        public DbSet<Event> Events { get; set; }

        public DbSet<NEventStatus> EventStatuses { get; set; }

        public DbSet<NCategory> Categories { get; set; }

        public DbSet<NTag> Tags { get; set; }

        public DbSet<Social> Socials { get; set; }

        public DbSet<SocialPlatformType> SocialPlatformTypes { get; set; }

        public DbSet<Interaction> Interactions { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupItem> GroupItems { get; set; }

        public DbSet<GroupItemType> GroupItemTypes { get; set; }

        public DbSet<EventTag> EventTags { get; set; }

        public DbSet<EventSocial> EventSocials { get; set; }

        public DbSet<GroupItemSocial> GroupItemSocials { get; set; }

        public DbSet<GroupItemVote> GroupItemVotes { get; set; }
        
        public EventContext(DbContextOptions<EventContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // added parent 
            modelBuilder.Entity<Group>()
                        .HasOne(g => g.GroupParent)
                        .WithMany(g => g.ChildGroups)
                        .HasForeignKey(g => g.GroupParentId);
        }
    }
}