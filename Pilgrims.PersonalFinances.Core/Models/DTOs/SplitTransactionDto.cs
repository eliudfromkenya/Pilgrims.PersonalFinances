namespace Pilgrims.PersonalFinances.Core.Models.DTOs;

/// <summary>
/// DTO for split transactions
/// </summary>
public class SplitTransactionDto
{
    public string TransactionId { get; set; } = "";
    public decimal Amount { get; set; }
    public string CategoryId { get; set; } = "";
    public string Description { get; set; } = "";
}