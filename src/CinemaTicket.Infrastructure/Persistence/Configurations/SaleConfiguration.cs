using CinemaTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaTicket.Infrastructure.Persistence.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.ReservationId)
            .HasColumnName("reservation_id")
            .IsRequired();

        builder.Property(s => s.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(s => s.PaymentMethod)
            .HasColumnName("payment_method")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.TransactionId)
            .HasColumnName("transaction_id")
            .HasMaxLength(100);

        builder.Property(s => s.TotalAmount)
            .HasColumnName("total_amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(s => s.PaidAt)
            .HasColumnName("paid_at")
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("idx_sales_user_id");

        // Check constraints
        builder.ToTable(t => t.HasCheckConstraint("chk_sale_amount", "total_amount > 0"));
    }
}
