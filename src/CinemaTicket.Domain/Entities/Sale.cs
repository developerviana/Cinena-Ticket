namespace CinemaTicket.Domain.Entities;

public class Sale : BaseEntity
{
    public Guid ReservationId { get; set; }
    public Guid UserId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime PaidAt { get; set; }

    // Navigation properties
    public virtual Reservation Reservation { get; set; } = null!;
}
