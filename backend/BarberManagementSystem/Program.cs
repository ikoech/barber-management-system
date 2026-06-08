using System.Text;
using BarberManagementSystem.Configuration;
using BarberManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//  Load JwtSettings FIRST
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

//  Add controllers
builder.Services.AddControllers();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
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
