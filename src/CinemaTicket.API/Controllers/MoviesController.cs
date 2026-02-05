using CinemaTicket.Domain.Entities;
using CinemaTicket.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CinemaTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MoviesController : ControllerBase
{
    private readonly CinemaTicketDbContext _context;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(CinemaTicketDbContext context, ILogger<MoviesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os filmes disponíveis
    /// </summary>
    /// <returns>Lista de filmes</returns>
    /// <response code="200">Retorna a lista de filmes</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todos os filmes",
        Description = "Retorna todos os filmes cadastrados no sistema",
        Tags = new[] { "Movies" }
    )]
    [SwaggerResponse(200, "Lista de filmes retornada com sucesso", typeof(IEnumerable<Movie>))]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
    {
        try
        {
            var movies = await _context.Movies
                .OrderBy(m => m.Title)
                .ToListAsync();

            return Ok(movies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar filmes");
            return StatusCode(500, new { error = "Erro ao buscar filmes" });
        }
    }

    /// <summary>
    /// Busca um filme específico por ID
    /// </summary>
    /// <param name="id">ID do filme</param>
    /// <returns>Dados do filme</returns>
    /// <response code="200">Filme encontrado</response>
    /// <response code="404">Filme não encontrado</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Busca filme por ID",
        Description = "Retorna os detalhes de um filme específico",
        Tags = new[] { "Movies" }
    )]
    [SwaggerResponse(200, "Filme encontrado", typeof(Movie))]
    [SwaggerResponse(404, "Filme não encontrado")]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult<Movie>> GetMovie(int id)
    {
        try
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound(new { error = $"Filme com ID {id} não encontrado" });
            }

            return Ok(movie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar filme {MovieId}", id);
            return StatusCode(500, new { error = "Erro ao buscar filme" });
        }
    }

    /// <summary>
    /// Busca filmes em cartaz
    /// </summary>
    /// <returns>Lista de filmes em cartaz</returns>
    /// <response code="200">Lista de filmes em cartaz</response>
    [HttpGet("now-showing")]
    [SwaggerOperation(
        Summary = "Filmes em cartaz",
        Description = "Retorna apenas os filmes que estão em cartaz atualmente",
        Tags = new[] { "Movies" }
    )]
    [SwaggerResponse(200, "Lista de filmes em cartaz", typeof(IEnumerable<Movie>))]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetNowShowing()
    {
        try
        {
            var movies = await _context.Movies
                .Where(m => m.IsActive)
                .OrderBy(m => m.Title)
                .ToListAsync();

            return Ok(movies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar filmes em cartaz");
            return StatusCode(500, new { error = "Erro ao buscar filmes em cartaz" });
        }
    }

    /// <summary>
    /// Busca sessões disponíveis para um filme
    /// </summary>
    /// <param name="id">ID do filme</param>
    /// <returns>Lista de sessões do filme</returns>
    /// <response code="200">Sessões encontradas</response>
    /// <response code="404">Filme não encontrado</response>
    [HttpGet("{id}/sessions")]
    [SwaggerOperation(
        Summary = "Sessões de um filme",
        Description = "Retorna todas as sessões disponíveis para um filme específico",
        Tags = new[] { "Movies" }
    )]
    [SwaggerResponse(200, "Sessões do filme", typeof(IEnumerable<Session>))]
    [SwaggerResponse(404, "Filme não encontrado")]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult<IEnumerable<Session>>> GetMovieSessions(int id)
    {
        try
        {
            var movieExists = await _context.Movies.AnyAsync(m => m.Id == id);
            if (!movieExists)
            {
                return NotFound(new { error = $"Filme com ID {id} não encontrado" });
            }

            var sessions = await _context.Sessions
                .Include(s => s.Room)
                .Where(s => s.MovieId == id && s.StartTime > DateTime.UtcNow)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return Ok(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessões do filme {MovieId}", id);
            return StatusCode(500, new { error = "Erro ao buscar sessões" });
        }
    }
}
