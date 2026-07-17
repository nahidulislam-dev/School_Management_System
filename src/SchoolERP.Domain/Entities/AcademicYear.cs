using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents an academic year/session (e.g. 2025-2026).</summary>
public class AcademicYear : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
}
