namespace CinemaTicket.Domain.Entities;

public class ReservationItem : BaseEntity
{
    public Guid ReservationId { get; set; }
    public Guid SeatId { get; set; }
    public decimal Price { get; set; }

    // Navigation properties
    public virtual Reservation Reservation { get; set; } = null!;
    public virtual Seat Seat { get; set; } = null!;
}
