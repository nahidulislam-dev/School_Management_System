namespace SchoolERP.Application.Features.FeeStructure.DTOs;

/// <summary>Input model for creating a new FeeStructure record.</summary>
public class CreateFeeStructureDto
{
    public int ClassId { get; set; }
    public int FeeTypeId { get; set; }
    public decimal Amount { get; set; }
}
