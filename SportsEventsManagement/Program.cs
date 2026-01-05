using Microsoft.AspNetCore.Authentication.JwtBearer; // [NEW] Needed for JWT
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;               // [NEW] Needed for Security Keys
using SportsEventsManagement.Data;
using SportsEventsManagement.Models;                // [NEW] Needed for User model
using System.Text;                                  // [NEW] Needed for Encoding
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Controller & JSON Configuration
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Fix for Infinite Loops (Match -> Team -> Match...)
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 2. Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register AuthService
builder.Services.AddScoped<SportsEventsManagement.Services.AuthService>();

// 4. [NEW] Configure JWT Authentication
// This reads the "Jwt" settings we added to appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// 5. CORS Configuration (Allow Blazor)
builder.Services.AddCors(options =>
{
    // In the AddCors section:
    options.AddPolicy("AllowBlazor", policy =>
    {
        // Change 7265 to 8001
        policy.WithOrigins("https://localhost:8001")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. [NEW] SEED DATA: Create a Default Admin User
// This runs every time the app starts to ensure you always have an admin.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Ensure the database is created (optional, depends on if you use Migrations)
        // context.Database.EnsureCreated(); 

        // Check if an Admin already exists
        if (!context.Utilisateurs.Any(u => u.Role == "Admin"))
        {
            Console.WriteLine("Creating Default Admin User...");
            context.Utilisateurs.Add(new Utilisateur
            {
                Nom = "Super Admin",
                Email = "admin@sportsevents.com",
                // Initial password: "Admin123!" (Hashed)
                MotDePasse = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                Telephone = "0000000000"
            });
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazor");

// 7. [NEW] Enable Authentication Middleware
// IMPORTANT: UseAuthentication must come BEFORE UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();