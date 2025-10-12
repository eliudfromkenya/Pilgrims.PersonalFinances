namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    public enum InsuranceStatus
    {
        Active = 1,
        Inactive = 2,
        Expired = 3,
        Cancelled = 4,
        Suspended = 5,
        PendingRenewal = 6,
        Lapsed = 7
    }

    public enum ClaimStatus
    {
        Submitted = 1,
        UnderReview = 2,
        InvestigationInProgress = 3,
        AdditionalDocumentsRequired = 4,
        Approved = 5,
        PartiallyApproved = 6,
        Rejected = 7,
        Settled = 8,
        Closed = 9,
        Cancelled = 10
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
        Overdue = 5,
        PartiallyPaid = 6,
        Refunded = 7
    }
}
