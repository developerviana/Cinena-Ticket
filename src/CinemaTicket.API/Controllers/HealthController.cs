using CinemaTicket.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CinemaTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly CinemaTicketDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(CinemaTicketDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Health check básico da API
    /// </summary>
    /// <returns>Status da aplicação</returns>
    /// <response code="200">API está saudável</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Health Check",
        Description = "Verifica se a API está online e respondendo",
        Tags = new[] { "Health" }
    )]
    [SwaggerResponse(200, "API está saudável")]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        });
    }

    /// <summary>
    /// Health check detalhado com verificação de dependências
    /// </summary>
    /// <returns>Status detalhado da aplicação e suas dependências</returns>
    /// <response code="200">Todos os serviços estão saudáveis</response>
    /// <response code="503">Algum serviço está indisponível</response>
    [HttpGet("detailed")]
    [SwaggerOperation(
        Summary = "Health Check Detalhado",
        Description = "Verifica o status da API e todas as suas dependências (banco de dados, cache, etc)",
        Tags = new[] { "Health" }
    )]
    [SwaggerResponse(200, "Todos os serviços estão saudáveis")]
    [SwaggerResponse(503, "Algum serviço está indisponível")]
    public async Task<IActionResult> GetDetailed()
    {
        var healthStatus = new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            checks = new
            {
                database = await CheckDatabase(),
                // redis = await CheckRedis(), // Implementar quando tiver Redis configurado
                // rabbitmq = await CheckRabbitMQ() // Implementar quando tiver RabbitMQ configurado
            }
        };

        var hasUnhealthyServices = healthStatus.checks.database.status != "healthy";

        if (hasUnhealthyServices)
        {
            return StatusCode(503, healthStatus);
        }

        return Ok(healthStatus);
    }

    private async Task<object> CheckDatabase()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
            {
                return new { status = "unhealthy", message = "Cannot connect to database" };
            }

            var moviesCount = await _context.Movies.CountAsync();
            var sessionsCount = await _context.Sessions.CountAsync();

            return new
            {
                status = "healthy",
                message = "Database is connected",
                stats = new
                {
                    movies = moviesCount,
                    sessions = sessionsCount
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return new { status = "unhealthy", message = ex.Message };
        }
    }
}
