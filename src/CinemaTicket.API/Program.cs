using CinemaTicket.Infrastructure.Persistence;
using CinemaTicket.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CinemaTicketDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Cinema Ticket API v1");
    });
}

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CinemaTicketDbContext>();
    
    try
    {
        // Apply pending migrations
        await dbContext.Database.MigrateAsync();
        
        // Seed database
        await DatabaseSeeder.SeedAsync(dbContext);
        
        app.Logger.LogInformation("? Database initialized successfully!");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "? Error initializing database");
        throw;
    }
}

app.UseHttpsRedirection();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck");

app.Run();
