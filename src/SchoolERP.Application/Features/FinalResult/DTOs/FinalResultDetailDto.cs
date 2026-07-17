namespace SchoolERP.Application.Features.FinalResult.DTOs;

/// <summary>Subject-wise breakdown row within a <see cref="FinalResultDto"/>.</summary>
public class FinalResultDetailDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public decimal FinalMarks { get; set; }
    public string FinalGradeLabel { get; set; } = string.Empty;
    public decimal FinalGradePoint { get; set; }
}
