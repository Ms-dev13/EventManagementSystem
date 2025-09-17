using EventManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Data
{
    public class AppDbContext:DbContext
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        { 
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserRole (many-to-many)
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict); // <-- restrict

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserRegistration (many-to-many)
            modelBuilder.Entity<UserRegistration>()
                .HasKey(ur => new { ur.UserId, ur.EventId });

            modelBuilder.Entity<UserRegistration>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict); // <-- restrict to avoid cascades

            modelBuilder.Entity<UserRegistration>()
                .HasOne(ur => ur.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(ur => ur.EventId)
                .OnDelete(DeleteBehavior.Cascade); // ok: Event -> Registrations cascade

            // Seeding Admin Role 
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Organizer" },
                new Role { Id = 3, Name = "Attendee" }
            );
            modelBuilder.Entity<User>().HasData(
                new User { 
                    Id = 1, 
                    Name = "Admin", 
                    Email = "envzaa@gmail.com", 
                    Password = "$2a$11$/c9i2baslYsvxpyWMgLdy.PlxRfNS5sgoIcjRCzYx8Nczp2Y/Sjzy", 
                    isBlocked = false }
                );
            modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = 1, RoleId = 1 });
        } 

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!; 
        public DbSet<UserRegistration> UserRegistrations { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;


    }
}
