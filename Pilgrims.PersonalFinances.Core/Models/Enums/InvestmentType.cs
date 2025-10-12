namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Represents types of investments supported by the system
    /// </summary>
    public enum InvestmentType
    {
        Stock = 0,
        Bond = 1,
        MutualFund = 2,
        ETF = 3,
        RealEstate = 4,
        Crypto = 5,
        Commodity = 6,
        CashEquivalent = 7,
        Other = 99
    }
}