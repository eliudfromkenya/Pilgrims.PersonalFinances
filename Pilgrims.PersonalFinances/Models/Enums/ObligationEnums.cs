namespace Pilgrims.PersonalFinances.Models.Enums
{
    public enum ObligationType
    {
        // Loan Management
        Mortgage = 1,
        PersonalLoan = 2,
        StudentLoan = 3,
        AutoLoan = 4,
        BusinessLoan = 5,
        
        // Credit Card Debt
        CreditCard = 6,
        
        // Subscription Services
        MonthlySubscription = 7,
        AnnualSubscription = 8,
        Membership = 9,
        
        // Recurring Commitments
        CharitableDonation = 10,
        MembershipFee = 11,
        ServiceContract = 12,
        
        // Tax Obligations
        EstimatedTaxPayment = 13,
        PropertyTax = 14,
        TaxLiability = 15,
        
        // Group Financial Obligations (existing)
        GroupWelfare = 16,
        Chama = 17,
        Sacco = 18,
        CooperativeSociety = 19,
        SavingsGroup = 20,
        InvestmentClub = 21,
        CreditUnion = 22,
        MicrofinanceGroup = 23,
        TableBanking = 24,
        RotatingSavings = 25,
        ProfessionalAssociation = 26,
        TradeUnion = 27,
        CommunityGroup = 28,
        ReligiousGroup = 29,
        
        // Medical and Insurance
        MedicalDebt = 30,
        InsurancePremium = 31,
        
        // Utilities and Services
        UtilityBill = 32,
        InternetService = 33,
        PhoneService = 34,
        
        Other = 99
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