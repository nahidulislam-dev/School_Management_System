using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a fee payment transaction made by/for a student.</summary>
public class FeeCollection : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public DateTime PaymentDate { get; set; }
    public string? TransactionId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string ReceiptNo { get; set; } = string.Empty;

    public int FeeTypeId { get; set; }
    public FeeType? FeeType { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Discount { get; set; }
    public decimal Fine { get; set; }

    public PaymentStatus Status { get; set; }
}
