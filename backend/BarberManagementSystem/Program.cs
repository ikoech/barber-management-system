using BarberManagementSystem.Configuration;
using BarberManagementSystem.DTOs.Services;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//  Load JwtSettings FIRST
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

//  Add controllers
builder.Services.AddControllers();
builder.Services.AddScoped<AvailabilityService>();
builder.Services.AddScoped<BookingEngine>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<AdminBookingService>();
//  Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Database
builder.Services.AddDatabase(builder.Configuration);

//  Authentication
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

            // ⭐ CRITICAL: This MUST match your JWT payload claim name
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
    });

//  Authorization - role-based
builder.Services.AddAuthorization(options =>
{
    // Admin only
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Barber or Admin
    options.AddPolicy("BarberOrAdmin", policy =>
        policy.RequireRole("Barber", "Admin"));

    // Customer or Admin
    options.AddPolicy("CustomerOrAdmin", policy =>
        policy.RequireRole("Customer", "Admin"));
});

//  Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

//  Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//  Auth middleware
app.UseAuthentication();
app.UseAuthorization();

//  Map controllers
app.MapControllers();

app.Run();
