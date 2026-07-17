namespace SchoolERP.Application.Features.FeeType.DTOs;

/// <summary>Read model returned to clients for a FeeType record.</summary>
public class FeeTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; }
    public bool IsRecurring { get; set; }
}
