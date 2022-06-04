using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QLESS.Core.Entities;
using System.Linq;

namespace QLESS.Core.Data.EntityFramework
{
    public class EntityFrameworkDbContext : DbContext, IEntityFrameworkDbContext
    {
        // Constructors
        public EntityFrameworkDbContext(DbContextOptions options) : base(options) { }

        // Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity(typeof(Card)).ToTable(nameof(Card));
            modelBuilder.Entity(typeof(CardType)).ToTable(nameof(CardType));
            modelBuilder.Entity(typeof(Privilege)).ToTable(nameof(Privilege));
            modelBuilder.Entity(typeof(PrivilegeCard)).ToTable(nameof(PrivilegeCard));
            modelBuilder.Entity(typeof(Trip)).ToTable(nameof(Trip));

            modelBuilder
                .Entity<CardTypePrivilege>()
                .HasKey(c => new { c.CardTypeId, c.PrivilegeId });

            modelBuilder
                .Entity<CardTypePrivilege>()
                .HasOne(cp => cp.CardType)
                .WithMany(c => c.CardTypePrivileges)
                .HasForeignKey(cp => cp.CardTypeId);

            modelBuilder
                .Entity<CardTypePrivilege>()
                .HasOne(cp => cp.Privilege)
                .WithMany(p => p.CardTypePrivileges)
                .HasForeignKey(cp => cp.PrivilegeId);

            modelBuilder
                .Entity<Card>()
                .HasOne(c => c.Type)
                .WithMany(c => c.Cards)
                .IsRequired();

            modelBuilder
                .Entity<Card>()
                .Property(c => c.Number)
                .IsRequired();

            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }
        }
        public void DetachAllEntities()
        {
            var changedEntriesCopy = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted
                )
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}
