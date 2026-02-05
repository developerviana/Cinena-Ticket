using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(CinemaTicketDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Movies
        if (!await context.Movies.AnyAsync())
        {
            var movies = new List<Movie>
            {
                new Movie
                {
                    Title = "Oppenheimer",
                    Description = "História de J. Robert Oppenheimer e o desenvolvimento da bomba atômica",
                    DurationMinutes = 180,
                    Genre = "Drama",
                    Rating = "14"
                },
                new Movie
                {
                    Title = "Barbie",
                    Description = "Barbie vive em Barbieland, um lugar perfeito para ser perfeita",
                    DurationMinutes = 114,
                    Genre = "Comédia",
                    Rating = "Livre"
                },
                new Movie
                {
                    Title = "The Batman",
                    Description = "Batman investiga uma série de assassinatos em Gotham City",
                    DurationMinutes = 176,
                    Genre = "Ação",
                    Rating = "14"
                },
                new Movie
                {
                    Title = "Dune: Part Two",
                    Description = "Paul Atreides une forças com Chani e os Fremen",
                    DurationMinutes = 166,
                    Genre = "Ficção Científica",
                    Rating = "12"
                }
            };

            await context.Movies.AddRangeAsync(movies);
            await context.SaveChangesAsync();
        }

        // Seed Rooms
        if (!await context.Rooms.AnyAsync())
        {
            var rooms = new List<Room>
            {
                new Room { RoomNumber = "A1", TotalSeats = 100 },
                new Room { RoomNumber = "A2", TotalSeats = 80 },
                new Room { RoomNumber = "B1", TotalSeats = 120 },
                new Room { RoomNumber = "B2", TotalSeats = 60 }
            };

            await context.Rooms.AddRangeAsync(rooms);
            await context.SaveChangesAsync();
        }

        // Seed Sessions
        if (!await context.Sessions.AnyAsync())
        {
            var movies = await context.Movies.ToListAsync();
            var rooms = await context.Rooms.ToListAsync();
            var sessions = new List<Session>();

            foreach (var movie in movies.Take(2)) // First 2 movies
            {
                foreach (var room in rooms.Take(2)) // First 2 rooms
                {
                    for (int day = 1; day <= 3; day++)
                    {
                        var startTime = DateTime.UtcNow.AddDays(day).Date.AddHours(19);
                        var session = new Session
                        {
                            MovieId = movie.Id,
                            RoomId = room.Id,
                            StartTime = startTime,
                            EndTime = startTime.AddMinutes(movie.DurationMinutes),
                            TicketPrice = movie.Title == "Oppenheimer" ? 35.00m : 30.00m,
                            Status = SessionStatus.Scheduled
                        };
                        sessions.Add(session);
                    }
                }
            }

            await context.Sessions.AddRangeAsync(sessions);
            await context.SaveChangesAsync();

            // Seed Seats for each session
            var allSessions = await context.Sessions.Include(s => s.Room).ToListAsync();
            var seats = new List<Seat>();

            foreach (var session in allSessions)
            {
                for (int i = 0; i < 50; i++) // 50 seats per session
                {
                    var row = (char)('A' + (i / 10));
                    var number = (i % 10) + 1;
                    seats.Add(new Seat
                    {
                        SessionId = session.Id,
                        SeatNumber = $"{row}{number:00}",
                        Status = SeatStatus.Available
                    });
                }
            }

            await context.Seats.AddRangeAsync(seats);
            await context.SaveChangesAsync();
        }
    }
}
