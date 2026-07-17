using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class FeeTypeConfiguration : IEntityTypeConfiguration<FeeType>
{
    public void Configure(EntityTypeBuilder<FeeType> builder)
    {
        builder.ToTable("FeeTypes");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
               .IsUnique();

        builder.Property(x => x.DefaultAmount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.IsRecurring)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Relationships

        // FeeType -> FeeStructures
        builder.HasMany(x => x.FeeStructures)
               .WithOne(x => x.FeeType)
               .HasForeignKey(x => x.FeeTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        // FeeType -> FeeCollections
        builder.HasMany(x => x.FeeCollections)
               .WithOne(x => x.FeeType)
               .HasForeignKey(x => x.FeeTypeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}