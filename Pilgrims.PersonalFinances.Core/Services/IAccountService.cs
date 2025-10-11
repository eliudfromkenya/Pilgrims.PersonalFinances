using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Services
{
    public interface IAccountService
    {
        // Account CRUD operations
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<IEnumerable<Account>> GetAccountsAsync(); // Alias for GetAllAccountsAsync
        Task<Account?> GetAccountByIdAsync(string? id);
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> UpdateAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(string? id);
        Task<bool> ArchiveAccountAsync(string? id);

        // Account operations
        Task<bool> CanDeleteAccountAsync(string? id);
        Task<bool> TransferFundsAsync(string? fromAccountId, string? toAccountId, decimal amount, string description, DateTime? transferDate);
        Task<bool> ReconcileAccountAsync(string? accountId, decimal bankBalance, DateTime reconciliationDate);
        Task<decimal> GetAccountBalanceAsync(string? accountId);
        Task<IEnumerable<Account>> GetAccountsByTypeAsync(AccountType accountType);
        Task<IEnumerable<Account>> GetActiveAccountsAsync();

        // Balance calculations
        Task<decimal> GetTotalBalanceAsync();
        Task<decimal> GetTotalBalanceByTypeAsync(AccountType accountType);
        Task<Dictionary<AccountType, decimal>> GetBalancesByTypeAsync();

        // Account validation
        Task<bool> ValidateAccountNameAsync(string name, string? excludeAccountId = null);
        Task<bool> ValidateAccountNumberAsync(string accountNumber, string? excludeAccountId = null);
        Task<bool> HasTransactionsAsync(string accountId);
    }
}
