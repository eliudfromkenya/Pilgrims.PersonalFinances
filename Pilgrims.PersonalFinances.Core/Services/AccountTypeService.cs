using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Core.Data;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Services
{
    public class AccountTypeService : IAccountTypeService
    {
        private readonly PersonalFinanceContext _db;

        public AccountTypeService(PersonalFinanceContext db)
        {
            _db = db;
        }

        public async Task<List<AccountTypeDefinition>> GetActiveTypesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Set<AccountTypeDefinition>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<AccountTypeDefinition?> GetByEnumAsync(AccountType enumValue, CancellationToken cancellationToken = default)
        {
            var val = (int)enumValue;
            return await _db.Set<AccountTypeDefinition>()
                .FirstOrDefaultAsync(x => x.EnumValue == val, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<string> GetIconByEnumAsync(AccountType enumValue, CancellationToken cancellationToken = default)
        {
            var def = await GetByEnumAsync(enumValue, cancellationToken).ConfigureAwait(false);
            return def?.Icon ?? string.Empty;
        }

        public async Task<AccountTypeDefinition> AddAsync(AccountTypeDefinition definition, CancellationToken cancellationToken = default)
        {
            await _db.Set<AccountTypeDefinition>().AddAsync(definition, cancellationToken).ConfigureAwait(false);
            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return definition;
        }

        public async Task<AccountTypeDefinition> UpdateAsync(AccountTypeDefinition definition, CancellationToken cancellationToken = default)
        {
            _db.Set<AccountTypeDefinition>().Update(definition);
            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return definition;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existing = await _db.Set<AccountTypeDefinition>().FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
            if (existing == null) return false;
            _db.Set<AccountTypeDefinition>().Remove(existing);
            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
    }
}