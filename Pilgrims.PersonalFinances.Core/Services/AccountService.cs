using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Services
{
    public class AccountService : IAccountService
    {
        private readonly PersonalFinanceContext _context;

        public AccountService(PersonalFinanceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            return await GetAllAccountsAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(string? id)
        {
            return await _context.Accounts
                .Include(a => a.Transactions.Take(10)) // Include recent transactions
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            // Validate account name uniqueness
            if (await ValidateAccountNameAsync(account.Name))
            {
                throw new InvalidOperationException("An account with this name already exists.");
            }

            // Set initial values
            account.CreatedAt = DateTime.UtcNow;
            account.MarkAsDirty();
            account.CurrentBalance = account.InitialBalance;

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<Account> UpdateAccountAsync(Account account)
        {
            var existingAccount = await _context.Accounts.FindAsync(account.Id);
            if (existingAccount == null)
            {
                throw new InvalidOperationException("Account not found.");
            }

            // Check if account has transactions before allowing opening balance change
            var hasTransactions = await _context.Transactions.AnyAsync(t => t.AccountId == account.Id);
            if (hasTransactions && existingAccount.InitialBalance != account.InitialBalance)
            {
                throw new InvalidOperationException("Cannot modify opening balance for accounts with existing transactions.");
            }

            // Validate account name uniqueness (excluding current account)
            if (await ValidateAccountNameAsync(account.Name, account.Id))
            {
                throw new InvalidOperationException("An account with this name already exists.");
            }

            // Update properties
            existingAccount.Name = account.Name;
            existingAccount.Description = account.Description;
            existingAccount.AccountType = account.AccountType;
            existingAccount.Currency = account.Currency;
            existingAccount.ColorCode = account.ColorCode;
            existingAccount.Status = account.Status;
            existingAccount.AccountNumber = account.AccountNumber;
            existingAccount.BankName = account.BankName;
            existingAccount.CreditLimit = account.CreditLimit;
            existingAccount.InterestRate = account.InterestRate;
        existingAccount.StatementDate = account.StatementDate;
        existingAccount.DueDate = account.DueDate;
        existingAccount.MinimumPayment = account.MinimumPayment;
        existingAccount.BrokerName = account.BrokerName;
        existingAccount.AccountHolder = account.AccountHolder;
        existingAccount.MarkAsDirty();

            // Only update initial balance if no transactions exist
            if (!hasTransactions)
            {
                existingAccount.InitialBalance = account.InitialBalance;
                existingAccount.CurrentBalance = account.InitialBalance;
            }

            await _context.SaveChangesAsync();
            return existingAccount;
        }

        public async Task<bool> DeleteAccountAsync(string? id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return false;

            // Check if account can be deleted
            if (!await CanDeleteAccountAsync(id))
            {
                throw new InvalidOperationException("Cannot delete account with existing transactions. Archive it instead.");
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ArchiveAccountAsync(string? id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return false;

            account.Status = AccountStatus.Closed;
            account.MarkAsDirty();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanDeleteAccountAsync(string? id)
        {
            return !await _context.Transactions.AnyAsync(t => t.AccountId == id);
        }

        public async Task<bool> TransferFundsAsync(string? fromAccountId, string? toAccountId, decimal amount, string description, DateTime? transferDate)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
                var toAccount = await _context.Accounts.FindAsync(toAccountId);

                if (fromAccount == null || toAccount == null)
                    return false;

                // Check if from account can perform the transaction
                if (!fromAccount.CanPerformTransaction(amount, TransactionType.Expense))
                    return false;

                // Update balances
                fromAccount.UpdateBalance(-amount, TransactionType.Expense);
                toAccount.UpdateBalance(amount, TransactionType.Income);

                // Create transfer transactions
                var transferOut = new Transaction
                {
                    AccountId = fromAccountId.ToString(),
                    Amount = -amount,
                    Type = TransactionType.Transfer,
                    Description = $"Transfer to {toAccount.Name}: {description}",
                    Date = DateTime.UtcNow
                };

                var transferIn = new Transaction
                {
                    AccountId = toAccountId.ToString(),
                    Amount = amount,
                    Type = TransactionType.Transfer,
                    Description = $"Transfer from {fromAccount.Name}: {description}",
                    Date = DateTime.UtcNow
                };

                _context.Transactions.AddRange(transferOut, transferIn);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> ReconcileAccountAsync(string? accountId, decimal bankBalance, DateTime reconciliationDate)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return false;

            account.Reconcile(bankBalance, reconciliationDate);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetAccountBalanceAsync(string? accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            return account?.CurrentBalance ?? 0;
        }

        public async Task<IEnumerable<Account>> GetAccountsByTypeAsync(AccountType accountType)
        {
            return await _context.Accounts
                .Where(a => a.AccountType == accountType)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetActiveAccountsAsync()
        {
            return await _context.Accounts
                .Where(a => a.Status == AccountStatus.Active)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            return await _context.Accounts
                .Where(a => a.Status == AccountStatus.Active)
                .SumAsync(a => a.CurrentBalance);
        }

        public async Task<decimal> GetTotalBalanceByTypeAsync(AccountType accountType)
        {
            return await _context.Accounts
                .Where(a => a.AccountType == accountType && a.Status == AccountStatus.Active)
                .SumAsync(a => a.CurrentBalance);
        }

        public async Task<Dictionary<AccountType, decimal>> GetBalancesByTypeAsync()
        {
            return await _context.Accounts
                .Where(a => a.Status == AccountStatus.Active)
                .GroupBy(a => a.AccountType)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(a => a.CurrentBalance));
        }

        public async Task<bool> ValidateAccountNameAsync(string name, string? excludeAccountId = null)
        {
            var query = _context.Accounts.Where(a => a.Name.ToLower() == name.ToLower());
            
            if (!string.IsNullOrEmpty(excludeAccountId))
            {
                query = query.Where(a => a.Id != excludeAccountId);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ValidateAccountNumberAsync(string accountNumber, string? excludeAccountId = null)
        {
            if (string.IsNullOrEmpty(accountNumber)) return false;

            var query = _context.Accounts.Where(a => a.AccountNumber == accountNumber);
            
            if (!string.IsNullOrEmpty(excludeAccountId))
            {
                query = query.Where(a => a.Id != excludeAccountId);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasTransactionsAsync(string accountId)
        {
            return await _context.Transactions.AnyAsync(t => t.AccountId == accountId);
        }
    }
}