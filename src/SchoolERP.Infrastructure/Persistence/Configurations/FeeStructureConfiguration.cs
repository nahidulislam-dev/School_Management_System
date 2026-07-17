using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class FeeStructureConfiguration : IEntityTypeConfiguration<FeeStructure>
{
    public void Configure(EntityTypeBuilder<FeeStructure> builder)
    {
        builder.ToTable("FeeStructures");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Amount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Relationships

        // SchoolClass -> FeeStructures
        builder.HasOne(x => x.SchoolClass)
               .WithMany()
               .HasForeignKey(x => x.ClassId)
               .OnDelete(DeleteBehavior.Restrict);

        // FeeType -> FeeStructures
        builder.HasOne(x => x.FeeType)
               .WithMany(x => x.FeeStructures)
               .HasForeignKey(x => x.FeeTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.ClassId);
        builder.HasIndex(x => x.FeeTypeId);

        // Unique Constraint: one fee type per class only once
        builder.HasIndex(x => new { x.ClassId, x.FeeTypeId })
               .IsUnique();
    }
}