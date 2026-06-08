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
    }
}
