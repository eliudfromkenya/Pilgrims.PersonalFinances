namespace Pilgrims.PersonalFinances.Models.Enums
{
    public enum ObligationType
    {
        GroupWelfare = 1,
        Chama = 2,
        Sacco = 3,
        CooperativeSociety = 4,
        SavingsGroup = 5,
        InvestmentClub = 6,
        CreditUnion = 7,
        MicrofinanceGroup = 8,
        TableBanking = 9,
        RotatingSavings = 10,
        ProfessionalAssociation = 11,
        TradeUnion = 12,
        CommunityGroup = 13,
        ReligiousGroup = 14,
        Other = 15
    }

    public enum ObligationStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        Terminated = 4,
        Completed = 5,
        PendingApproval = 6,
        OnHold = 7,
        Defaulted = 8
    }

    public enum BenefitStatus
    {
        Pending = 1,
        Approved = 2,
        Processed = 3,
        Paid = 4,
        Rejected = 5,
        Cancelled = 6,
        OnHold = 7
    }
}