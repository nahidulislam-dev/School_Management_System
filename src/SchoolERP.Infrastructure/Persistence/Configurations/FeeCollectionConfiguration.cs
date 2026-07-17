using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class FeeCollectionConfiguration : IEntityTypeConfiguration<FeeCollection>
{
    public void Configure(EntityTypeBuilder<FeeCollection> builder)
    {
        builder.ToTable("FeeCollections");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.PaymentDate)
               .IsRequired();

        builder.Property(x => x.TransactionId)
               .HasMaxLength(100);

        builder.Property(x => x.PaymentMethod)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.ReceiptNo)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(x => x.ReceiptNo)
               .IsUnique();

        builder.Property(x => x.AmountDue)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.AmountPaid)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.Discount)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Fine)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Status)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Relationships

        // Student -> FeeCollections
        builder.HasOne(x => x.Student)
               .WithMany()
               .HasForeignKey(x => x.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        // FeeType -> FeeCollections
        builder.HasOne(x => x.FeeType)
               .WithMany(x => x.FeeCollections)
               .HasForeignKey(x => x.FeeTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.StudentId);
        builder.HasIndex(x => x.FeeTypeId);
        builder.HasIndex(x => new { x.StudentId, x.Year, x.Month });

        // Business Rule: One payment per student per fee type per month/year
        builder.HasIndex(x => new { x.StudentId, x.FeeTypeId, x.Month, x.Year })
               .IsUnique();
    }
}