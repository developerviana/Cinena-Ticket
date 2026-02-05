using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaTicket.Infrastructure.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.MovieId)
            .HasColumnName("movie_id")
            .IsRequired();

        builder.Property(s => s.RoomId)
            .HasColumnName("room_id")
            .IsRequired();

        builder.Property(s => s.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(s => s.EndTime)
            .HasColumnName("end_time")
            .IsRequired();

        builder.Property(s => s.TicketPrice)
            .HasColumnName("ticket_price")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => Enum.Parse<SessionStatus>(v, true))
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.MovieId)
            .HasDatabaseName("idx_sessions_movie_id");

        builder.HasIndex(s => s.RoomId)
            .HasDatabaseName("idx_sessions_room_id");

        builder.HasIndex(s => s.StartTime)
            .HasDatabaseName("idx_sessions_start_time");

        // Check constraints
        builder.ToTable(t => t.HasCheckConstraint("chk_session_time", "end_time > start_time"));
        builder.ToTable(t => t.HasCheckConstraint("chk_ticket_price", "ticket_price > 0"));

        // Relationships
        builder.HasMany(s => s.Seats)
            .WithOne(st => st.Session)
            .HasForeignKey(st => st.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Reservations)
            .WithOne(r => r.Session)
            .HasForeignKey(r => r.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
