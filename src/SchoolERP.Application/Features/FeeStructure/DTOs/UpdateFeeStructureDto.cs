namespace SchoolERP.Application.Features.FeeStructure.DTOs;

/// <summary>Input model for updating an existing FeeStructure record.</summary>
public class UpdateFeeStructureDto
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int FeeTypeId { get; set; }
    public decimal Amount { get; set; }
}
