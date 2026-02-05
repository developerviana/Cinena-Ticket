using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

public class Session : BaseEntity
{
    public Guid MovieId { get; set; }
    public Guid RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TicketPrice { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Scheduled;

    // Navigation properties
    public virtual Movie Movie { get; set; } = null!;
    public virtual Room Room { get; set; } = null!;
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
