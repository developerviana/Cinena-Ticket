using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

public class Seat : BaseEntity
{
    public Guid SessionId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public SeatStatus Status { get; set; } = SeatStatus.Available;
    public DateTime? ReservedAt { get; set; }
    public Guid? ReservedBy { get; set; }

    // Navigation properties
    public virtual Session Session { get; set; } = null!;
    public virtual ICollection<ReservationItem> ReservationItems { get; set; } = new List<ReservationItem>();
}
