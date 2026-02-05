using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaTicket.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.SessionId)
            .HasColumnName("session_id")
            .IsRequired();

        builder.Property(r => r.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => Enum.Parse<ReservationStatus>(v, true))
            .IsRequired();

        builder.Property(r => r.TotalAmount)
            .HasColumnName("total_amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(r => r.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("idx_reservations_user_id");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("idx_reservations_status");

        builder.HasIndex(r => r.ExpiresAt)
            .HasDatabaseName("idx_reservations_expires_at");

        // Check constraints
        builder.ToTable(t => t.HasCheckConstraint("chk_reservation_status",
            "status IN ('pending', 'confirmed', 'expired', 'cancelled')"));
        builder.ToTable(t => t.HasCheckConstraint("chk_total_amount", "total_amount > 0"));

        // Relationships
        builder.HasMany(r => r.ReservationItems)
            .WithOne(ri => ri.Reservation)
            .HasForeignKey(ri => ri.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Sale)
            .WithOne(s => s.Reservation)
            .HasForeignKey<Sale>(s => s.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
