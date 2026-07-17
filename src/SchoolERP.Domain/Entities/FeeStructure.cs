using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents the fee amount applicable for a fee type in a given class.</summary>
public class FeeStructure : BaseEntity
{
    public int ClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    public int FeeTypeId { get; set; }
    public FeeType? FeeType { get; set; }

    public decimal Amount { get; set; }
}
