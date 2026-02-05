using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaTicket.Infrastructure.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("seats");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.SessionId)
            .HasColumnName("session_id")
            .IsRequired();

        builder.Property(s => s.SeatNumber)
            .HasColumnName("seat_number")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => Enum.Parse<SeatStatus>(v, true))
            .IsRequired();

        builder.Property(s => s.ReservedAt)
            .HasColumnName("reserved_at");

        builder.Property(s => s.ReservedBy)
            .HasColumnName("reserved_by");

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.SessionId)
            .HasDatabaseName("idx_seats_session_id");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("idx_seats_status");

        builder.HasIndex(s => new { s.SessionId, s.SeatNumber })
            .IsUnique();

        // Check constraints
        builder.ToTable(t => t.HasCheckConstraint("chk_seat_status", 
            "status IN ('available', 'reserved', 'sold')"));

        // Relationships
        builder.HasMany(s => s.ReservationItems)
            .WithOne(ri => ri.Seat)
            .HasForeignKey(ri => ri.SeatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
