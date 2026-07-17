using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a category of fee (e.g. Tuition, Admission, Exam Fee).</summary>
public class FeeType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; }
    public bool IsRecurring { get; set; }

    public ICollection<FeeStructure> FeeStructures { get; set; } = new List<FeeStructure>();
    public ICollection<FeeCollection> FeeCollections { get; set; } = new List<FeeCollection>();
}
