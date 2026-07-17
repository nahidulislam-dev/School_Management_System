namespace SchoolERP.Domain.Enums;

/// <summary>
/// Attendance state of a student for a specific subject's mark entry
/// (distinct from day-to-day <see cref="AttendanceStatus"/>). Determines
/// whether <see cref="Entities.Result.MarksObtained"/> is meaningful.
/// </summary>
public enum MarkAttendanceStatus
{
    /// <summary>Student sat the exam; marks are entered normally.</summary>
    Present = 1,

    /// <summary>Student did not sit the exam. Marks must be 0.</summary>
    Absent = 2,

    /// <summary>Student was excused for a medical reason. Marks must be 0; typically excluded from pass/fail averaging by policy.</summary>
    Medical = 3,

    /// <summary>Result withheld (e.g. disciplinary/administrative hold). Marks must be 0.</summary>
    Withheld = 4,

    /// <summary>Only some components of the exam were completed. Marks reflect only what was assessed.</summary>
    Incomplete = 5
}
