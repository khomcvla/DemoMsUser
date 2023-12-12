using Microsoft.EntityFrameworkCore;
using DemoMsUser.Data.Models;

namespace DemoMsUser.Data
{
    public class DataContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);

            modelBuilder.Entity<User>()
                .Property<DateTime?>("DeletedAt")
                .IsRequired(false);
        }
    }
}
