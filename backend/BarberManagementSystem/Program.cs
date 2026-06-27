using BarberManagementSystem.Configuration;
using BarberManagementSystem.DTOs.Services;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
// Load JwtSettings FIRST
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

// Controllers
builder.Services.AddControllers();

// Services
builder.Services.AddScoped<AvailabilityService>();
builder.Services.AddScoped<BookingEngine>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<AdminBookingService>();
builder.Services.AddScoped<ServiceService>();
builder.Services.AddScoped<BarberService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<WorkingHoursService>();
builder.Services.AddScoped<WorkingHoursAvailabilityService>();
builder.Services.AddScoped<BreakService>();

builder.Services.AddScoped<AdminStatsService>();
builder.Services.AddScoped<DaysOffService>();
builder.Services.AddScoped<CalendarService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Authentication (JWT)
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)
            ),

            // MUST MATCH your JWT claim
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
    });

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("BarberOrAdmin", policy =>
        policy.RequireRole("Barber", "Admin"));

    options.AddPolicy("CustomerOrAdmin", policy =>
        policy.RequireRole("Customer", "Admin"));
});

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Auth middleware
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();
