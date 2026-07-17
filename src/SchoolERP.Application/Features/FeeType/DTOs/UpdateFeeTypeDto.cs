namespace SchoolERP.Application.Features.FeeType.DTOs;

/// <summary>Input model for updating an existing FeeType record.</summary>
public class UpdateFeeTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; }
    public bool IsRecurring { get; set; }
}
