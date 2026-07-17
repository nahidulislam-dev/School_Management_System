namespace SchoolERP.Application.Features.FeeStructure.DTOs;

/// <summary>Read model returned to clients for a FeeStructure record.</summary>
public class FeeStructureDto
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int FeeTypeId { get; set; }
    public decimal Amount { get; set; }
}
