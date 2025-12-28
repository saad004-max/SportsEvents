using Microsoft.EntityFrameworkCore;
using SportsEventsManagement.Data;

var builder = WebApplication.CreateBuilder(args);


// Fix for Infinite Loops (Match -> Team -> Match...)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Allow the Blazor Frontend to talk to this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7265") // The address from your screenshot
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddScoped<SportsEventsManagement.Services.AuthService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connection to the Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthorization();
app.MapControllers();

app.Run();