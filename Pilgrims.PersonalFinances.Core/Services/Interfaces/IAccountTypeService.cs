using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces
{
    public interface IAccountTypeService
    {
        Task<List<AccountTypeDefinition>> GetActiveTypesAsync(CancellationToken cancellationToken = default);
        Task<AccountTypeDefinition?> GetByEnumAsync(AccountType enumValue, CancellationToken cancellationToken = default);
        Task<string> GetIconByEnumAsync(AccountType enumValue, CancellationToken cancellationToken = default);
        Task<AccountTypeDefinition> AddAsync(AccountTypeDefinition definition, CancellationToken cancellationToken = default);
        Task<AccountTypeDefinition> UpdateAsync(AccountTypeDefinition definition, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}