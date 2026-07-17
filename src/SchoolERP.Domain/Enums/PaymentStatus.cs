namespace SchoolERP.Domain.Enums;

/// <summary>Payment status for fee collection records.</summary>
public enum PaymentStatus
{
    Due = 1,
    Partial = 2,
    Paid = 3,
    Cancelled = 4
}
