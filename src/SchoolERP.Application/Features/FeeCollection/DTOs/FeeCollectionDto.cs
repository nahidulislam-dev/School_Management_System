using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.FeeCollection.DTOs;

/// <summary>Read model returned to clients for a FeeCollection record.</summary>
public class FeeCollectionDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? TransactionId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string ReceiptNo { get; set; } = string.Empty;
    public int FeeTypeId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Discount { get; set; }
    public decimal Fine { get; set; }
    public PaymentStatus Status { get; set; }
}
