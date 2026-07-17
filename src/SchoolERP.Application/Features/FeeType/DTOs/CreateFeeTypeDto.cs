namespace SchoolERP.Application.Features.FeeType.DTOs;

/// <summary>Input model for creating a new FeeType record.</summary>
public class CreateFeeTypeDto
{
    public string Name { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; }
    public bool IsRecurring { get; set; }
}
