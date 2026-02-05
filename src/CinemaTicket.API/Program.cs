using CinemaTicket.Infrastructure.Persistence;
using CinemaTicket.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI Configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cinema Ticket API",
        Version = "v1",
        Description = "Sistema Distribuído de Venda de Ingressos de Cinema com controle de concorrência",
        Contact = new OpenApiContact
        {
            Name = "Cinema Ticket Team",
            Url = new Uri("https://github.com/developerviana/Cinena-Ticket")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Habilitar anotações do Swagger
    options.EnableAnnotations();
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CinemaTicketDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cinema Ticket API v1");
        options.RoutePrefix = string.Empty; // Swagger na raiz (https://localhost:5001/)
        options.DocumentTitle = "Cinema Ticket API - Documentation";
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
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

app.Run();
