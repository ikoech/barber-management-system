using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<WorkingHours> WorkingHours { get; set; }
        public DbSet<Break> Breaks { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.BarberProfile)
                .WithOne(b => b.User)
                .HasForeignKey<Barber>(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BARBER
            modelBuilder.Entity<Barber>()
                .HasMany(b => b.WorkingHours)
                .WithOne(wh => wh.Barber)
                .HasForeignKey(wh => wh.BarberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Barber>()
                .HasMany(b => b.Breaks)
                .WithOne(br => br.Barber)
                .HasForeignKey(br => br.BarberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Barber>()
                .HasMany(b => b.Bookings)
                .WithOne(bk => bk.Barber)
                .HasForeignKey(bk => bk.BarberId)
                .OnDelete(DeleteBehavior.Restrict);

            // SERVICE
            modelBuilder.Entity<Service>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.Service)
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // WORKING HOURS
            modelBuilder.Entity<WorkingHours>()
                .HasOne(wh => wh.Barber)
                .WithMany(b => b.WorkingHours)
                .HasForeignKey(wh => wh.BarberId)
                .OnDelete(DeleteBehavior.Cascade);

            // BREAKS
            modelBuilder.Entity<Break>()
                .HasOne(br => br.Barber)
                .WithMany(b => b.Breaks)
                .HasForeignKey(br => br.BarberId)
                .OnDelete(DeleteBehavior.Cascade);

            // BOOKINGS
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Barber)
                .WithMany(br => br.Bookings)
                .HasForeignKey(b => b.BarberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
