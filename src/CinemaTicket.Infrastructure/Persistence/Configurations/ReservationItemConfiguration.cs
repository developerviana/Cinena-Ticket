using CinemaTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaTicket.Infrastructure.Persistence.Configurations;

public class ReservationItemConfiguration : IEntityTypeConfiguration<ReservationItem>
{
    public void Configure(EntityTypeBuilder<ReservationItem> builder)
    {
        builder.ToTable("reservation_items");

        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(ri => ri.ReservationId)
            .HasColumnName("reservation_id")
            .IsRequired();

        builder.Property(ri => ri.SeatId)
            .HasColumnName("seat_id")
            .IsRequired();

        builder.Property(ri => ri.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(ri => ri.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ri => ri.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(ri => new { ri.ReservationId, ri.SeatId })
            .IsUnique();

        // Check constraints
        builder.ToTable(t => t.HasCheckConstraint("chk_item_price", "price > 0"));
    }
}
