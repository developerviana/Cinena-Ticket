using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

public class Reservation : BaseEntity
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public decimal TotalAmount { get; set; }
    public DateTime ExpiresAt { get; set; }

    // Navigation properties
    public virtual Session Session { get; set; } = null!;
    public virtual ICollection<ReservationItem> ReservationItems { get; set; } = new List<ReservationItem>();
    public virtual Sale? Sale { get; set; }
}
