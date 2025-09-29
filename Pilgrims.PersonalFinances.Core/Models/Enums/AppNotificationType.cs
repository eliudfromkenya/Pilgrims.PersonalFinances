namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Types of notifications in the system
    /// </summary>
    public enum AppNotificationType
    {
        BillReminder = 1,
        BudgetAlert = 2,
        DebtPayment = 3,
        IncomeExpectation = 4,
        ReconciliationReminder = 5,
        SystemAlert = 6,
        InsurancePremiumDue = 7,
        InsuranceClaimUpdate = 8,
        InsurancePolicyExpiry = 9,
        ObligationPaymentDue = 10,
        ObligationArrears = 11,
        ObligationBenefitAvailable = 12,
        GroupContributionReminder = 13,
        ScheduledTransactionDue = 14,
        BudgetExceeded = 15,
        LowBalance = 16,
        RecurringTransactionFailed = 17,
        SystemUpdate = 18
    }
}