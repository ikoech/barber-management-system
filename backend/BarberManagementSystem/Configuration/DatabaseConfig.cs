using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace BarberManagementSystem.Configuration;

public static class DatabaseConfig
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
    }
}
