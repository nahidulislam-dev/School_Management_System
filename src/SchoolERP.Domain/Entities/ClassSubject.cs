namespace SchoolERP.Domain.Entities;

/// <summary>Join entity mapping <see cref="SchoolClass"/> to <see cref="Subject"/>.</summary>
public class ClassSubject
{
    public int ClassId { get; set; }
    public SchoolClass SchoolClass { get; set; } = null!;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
}
