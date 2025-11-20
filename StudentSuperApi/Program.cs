using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json.Serialization;
using StudentSuperApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;
var environment = builder.Environment;

// 1. Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// 2. CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// 3. Controllers + JSON options (fix object cycle issue)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// JWT configuration - support multiple sources and provide a safe dev fallback
var jwtSection = configuration.GetSection("Jwt");

// Priority: ENV JWT_KEY -> appsettings / user-secrets "Jwt:Key"
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
             ?? jwtSection.GetValue<string>("Key");

// If running in Development and no key is provided, use a dev-only key and warn
if (string.IsNullOrWhiteSpace(jwtKey) && environment.IsDevelopment())
{
    jwtKey = "dev-secret-please-change-this-to-a-real-secret-32+chars";
    Console.WriteLine("WARNING: Using development JWT key. Replace with a secure secret via user-secrets or JWT_KEY env var.");
}

// In non-development, fail fast if key missing
if (string.IsNullOrWhiteSpace(jwtKey) && !environment.IsDevelopment())
{
    throw new InvalidOperationException(
        "JWT key missing. Set 'Jwt:Key' in appsettings/user-secrets or set environment variable 'JWT_KEY'.");
}

var jwtIssuer = jwtSection.GetValue<string>("Issuer");
var jwtAudience = jwtSection.GetValue<string>("Audience");
var jwtExpires = jwtSection.GetValue<int?>("ExpiresMinutes") ?? 60;

// Create JwtSettings instance from the resolved values (ensures controllers see the same key)
var jwtSettings = new JwtSettings
{
    Key = jwtKey,
    Issuer = jwtIssuer,
    Audience = jwtAudience,
    ExpiresMinutes = jwtExpires
};

// Register JwtSettings instance in DI so controllers receive the effective key used by authentication
builder.Services.AddSingleton(jwtSettings);

// Also bind configuration to IOptions<JwtSettings> for code that might use IOptions
builder.Services.Configure<JwtSettings>(jwtSection);

// Configure authentication using the resolved jwtKey
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
        ValidIssuer = jwtIssuer,
        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
        ValidAudience = jwtAudience,
        ValidateLifetime = true
    };
});

// Make all endpoints require authentication by default in non-development environments.
// In Development we skip applying a global fallback so Swagger/UI and anonymous testing work.
if (!environment.IsDevelopment())
{
    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });
}
else
{
    // In Development still register authorization so [Authorize] works on controllers/actions.
    builder.Services.AddAuthorization();
}

// 4. Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the app
var app = builder.Build();

// 5. Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
