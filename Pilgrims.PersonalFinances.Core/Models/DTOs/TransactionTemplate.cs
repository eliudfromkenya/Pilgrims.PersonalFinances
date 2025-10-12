using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs;

/// <summary>
/// DTO for transaction templates
/// </summary>
public class TransactionTemplate
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? CategoryId { get; set; }
    public string? Payee { get; set; }
    public string? Tags { get; set; }
}