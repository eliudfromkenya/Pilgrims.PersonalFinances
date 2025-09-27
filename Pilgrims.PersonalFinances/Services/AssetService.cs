using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Extensions;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services;

/// <summary>
/// Comprehensive service for asset management with CRUD operations, depreciation tracking, and reporting
/// </summary>
public class AssetService : IAssetService
{
    private readonly PersonalFinanceContext _context;
    private readonly INotificationService _notificationService;

    public AssetService(PersonalFinanceContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    #region Basic CRUD Operations - Assets

    public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Include(a => a.AssetRegister)
            .Include(a => a.InsurancePolicies)
            .Include(a => a.MaintenanceRecords)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Asset?> GetAssetByIdAsync(string id)
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Include(a => a.AssetRegister)
                .ThenInclude(ar => ar.Documents)
            .Include(a => a.InsurancePolicies)
            .Include(a => a.MaintenanceRecords)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Asset> CreateAssetAsync(Asset asset)
    {
        if (asset == null)
            throw new ArgumentNullException(nameof(asset));

        // Validate the asset
        if (!await ValidateAssetAsync(asset))
            throw new InvalidOperationException("Asset validation failed");

        // Check for unique serial number if provided
        if (!string.IsNullOrEmpty(asset.SerialNumber) && 
            !await IsSerialNumberUniqueAsync(asset.SerialNumber))
        {
            throw new InvalidOperationException("Serial number must be unique");
        }

        asset.Id = Guid.NewGuid().ToString();
        asset.CreatedAt = DateTime.UtcNow;
        asset.TouchUpdatedAt();

        // Set initial current value to purchase price if not specified
        if (asset.CurrentValue == 0)
            asset.CurrentValue = asset.PurchasePrice;

        _context.Assets.Add(asset);

        // Create corresponding AssetRegister entry
        var assetRegister = new AssetRegister
        {
            Id = Guid.NewGuid().ToString(),
            AssetId = asset.Id,
            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice,
            CreatedAt = DateTime.UtcNow
        };
        assetRegister.TouchUpdatedAt();

        _context.AssetRegisters.Add(assetRegister);

        await _context.SaveChangesAsync();

        // Send notification
        await _notificationService.CreateNotificationAsync(
            "New Asset Added",
            $"Asset '{asset.Name}' has been successfully added to your portfolio.",
            Pilgrims.PersonalFinances.Models.Enums.AppNotificationType.SystemAlert
        );

        return asset;
    }

    public async Task<Asset> UpdateAssetAsync(Asset asset)
    {
        if (asset == null)
            throw new ArgumentNullException(nameof(asset));

        var existingAsset = await _context.Assets.FindAsync(asset.Id);
        if (existingAsset == null)
            throw new InvalidOperationException("Asset not found");

        // Validate the asset
        if (!await ValidateAssetAsync(asset))
            throw new InvalidOperationException("Asset validation failed");

        // Check for unique serial number if changed
        if (!string.IsNullOrEmpty(asset.SerialNumber) && 
            asset.SerialNumber != existingAsset.SerialNumber &&
            !await IsSerialNumberUniqueAsync(asset.SerialNumber, asset.Id))
        {
            throw new InvalidOperationException("Serial number must be unique");
        }

        // Update properties
        existingAsset.Name = asset.Name;
        existingAsset.Description = asset.Description;
        existingAsset.AssetCategoryId = asset.AssetCategoryId;
        existingAsset.PurchaseDate = asset.PurchaseDate;
        existingAsset.PurchasePrice = asset.PurchasePrice;
        existingAsset.CurrentValue = asset.CurrentValue;
        existingAsset.SerialNumber = asset.SerialNumber;
        existingAsset.Model = asset.Model;
        existingAsset.Brand = asset.Brand;
        existingAsset.Location = asset.Location;
        existingAsset.Condition = asset.Condition;
        existingAsset.DepreciationMethod = asset.DepreciationMethod;
        existingAsset.DepreciationRate = asset.DepreciationRate;
        existingAsset.UsefulLifeYears = asset.UsefulLifeYears;
        existingAsset.SalvageValue = asset.SalvageValue;
        existingAsset.IsActive = asset.IsActive;
        existingAsset.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return existingAsset;
    }

    public async Task<bool> DeleteAssetAsync(string id)
    {
        if (!await CanDeleteAssetAsync(id))
            return false;

        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
            return false;

        // Soft delete by marking as inactive
        asset.IsActive = false;
        asset.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAssetsAsync(IEnumerable<string> ids)
    {
        var assets = await _context.Assets
            .Where(a => ids.Contains(a.Id))
            .ToListAsync();

        foreach (var asset in assets)
        {
            if (await CanDeleteAssetAsync(asset.Id))
            {
                asset.IsActive = false;
                asset.TouchUpdatedAt();
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Asset Filtering and Search

    public async Task<IEnumerable<Asset>> GetAssetsByCategoryAsync(string categoryId)
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.AssetCategoryId == categoryId && a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> GetActiveAssetsAsync()
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> GetDisposedAssetsAsync()
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => !a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> SearchAssetsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetActiveAssetsAsync();

        var term = searchTerm.ToLower();
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.IsActive && (
                a.Name.ToLower().Contains(term) ||
                a.Description.ToLower().Contains(term) ||
                a.Brand.ToLower().Contains(term) ||
                a.Model.ToLower().Contains(term) ||
                a.SerialNumber.ToLower().Contains(term) ||
                a.AssetCategory.Name.ToLower().Contains(term)
            ))
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> GetAssetsByLocationAsync(string location)
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.IsActive && a.Location.ToLower().Contains(location.ToLower()))
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> GetAssetsByConditionAsync(string condition)
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.IsActive && a.Condition.ToLower() == condition.ToLower())
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> GetAssetsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.IsActive && a.PurchaseDate >= startDate && a.PurchaseDate <= endDate)
            .OrderBy(a => a.PurchaseDate)
            .ToListAsync();
    }

    #endregion

    #region Asset Categories

    public async Task<IEnumerable<AssetCategory>> GetAllAssetCategoriesAsync()
    {
        return await _context.AssetCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<AssetCategory?> GetAssetCategoryByIdAsync(string id)
    {
        return await _context.AssetCategories
            .Include(c => c.Assets)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<AssetCategory> CreateAssetCategoryAsync(AssetCategory category)
    {
        category.Id = Guid.NewGuid().ToString();
        category.CreatedAt = DateTime.UtcNow;
        category.TouchUpdatedAt();

        _context.AssetCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<AssetCategory> UpdateAssetCategoryAsync(AssetCategory category)
    {
        var existing = await _context.AssetCategories.FindAsync(category.Id);
        if (existing == null)
            throw new InvalidOperationException("Category not found");

        existing.Name = category.Name;
        existing.Description = category.Description;
        existing.Icon = category.Icon;
        existing.Color = category.Color;
        existing.DefaultDepreciationMethod = category.DefaultDepreciationMethod;
        existing.DefaultDepreciationRate = category.DefaultDepreciationRate;
        existing.DefaultUsefulLifeYears = category.DefaultUsefulLifeYears;
        existing.IsActive = category.IsActive;
        existing.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAssetCategoryAsync(string id)
    {
        var category = await _context.AssetCategories.FindAsync(id);
        if (category == null)
            return false;

        // Check if category has assets
        var hasAssets = await _context.Assets.AnyAsync(a => a.AssetCategoryId == id && a.IsActive);
        if (hasAssets)
            throw new InvalidOperationException("Cannot delete category with active assets");

        category.IsActive = false;
        category.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Asset Register Operations

    public async Task<AssetRegister?> GetAssetRegisterByAssetIdAsync(string assetId)
    {
        return await _context.AssetRegisters
            .Include(ar => ar.Asset)
            .Include(ar => ar.Documents)
            .FirstOrDefaultAsync(ar => ar.AssetId == assetId);
    }

    public async Task<AssetRegister> CreateOrUpdateAssetRegisterAsync(AssetRegister assetRegister)
    {
        var existing = await _context.AssetRegisters
            .FirstOrDefaultAsync(ar => ar.AssetId == assetRegister.AssetId);

        if (existing != null)
        {
            existing.Status = assetRegister.Status;
            existing.MaintenanceNotes = assetRegister.MaintenanceNotes;
            existing.TouchUpdatedAt();
            await _context.SaveChangesAsync();
            return existing;
        }

        assetRegister.Id = Guid.NewGuid().ToString();
        assetRegister.CreatedAt = DateTime.UtcNow;
        assetRegister.TouchUpdatedAt();

        _context.AssetRegisters.Add(assetRegister);
        await _context.SaveChangesAsync();
        return assetRegister;
    }

    public async Task<bool> DeleteAssetRegisterAsync(string id)
    {
        var assetRegister = await _context.AssetRegisters.FindAsync(id);
        if (assetRegister == null)
            return false;

        _context.AssetRegisters.Remove(assetRegister);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Asset Maintenance

    public async Task<IEnumerable<AssetMaintenance>> GetMaintenanceRecordsByAssetAsync(string assetId)
    {
        return await _context.AssetMaintenances
            .Where(m => m.AssetId == assetId)
            .OrderByDescending(m => m.ServiceDate)
            .ToListAsync();
    }

    public async Task<AssetMaintenance> CreateMaintenanceRecordAsync(AssetMaintenance maintenance)
    {
        maintenance.Id = Guid.NewGuid().ToString();
        maintenance.CreatedAt = DateTime.UtcNow;
        maintenance.TouchUpdatedAt();

        _context.AssetMaintenances.Add(maintenance);
        await _context.SaveChangesAsync();

        // Create notification for maintenance record
        var asset = await _context.Assets.FindAsync(maintenance.AssetId);
        if (asset != null)
        {
            await _notificationService.CreateNotificationAsync(
                "Maintenance Record Added",
                $"Maintenance record for '{asset.Name}' has been added.",
                Pilgrims.PersonalFinances.Models.Enums.AppNotificationType.SystemAlert
            );
        }

        return maintenance;
    }

    public async Task<AssetMaintenance> UpdateMaintenanceRecordAsync(AssetMaintenance maintenance)
    {
        var existing = await _context.AssetMaintenances.FindAsync(maintenance.Id);
        if (existing == null)
            throw new InvalidOperationException("Maintenance record not found");

        existing.ServiceType = maintenance.ServiceType;
        existing.Description = maintenance.Description;
        existing.ServiceDate = maintenance.ServiceDate;
        existing.Cost = maintenance.Cost;
        existing.ServiceProvider = maintenance.ServiceProvider;
        existing.NextServiceDue = maintenance.NextServiceDue;
        existing.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteMaintenanceRecordAsync(string id)
    {
        var maintenance = await _context.AssetMaintenances.FindAsync(id);
        if (maintenance == null)
            return false;

        _context.AssetMaintenances.Remove(maintenance);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AssetMaintenance>> GetUpcomingMaintenanceAsync(int daysAhead = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);
        return await _context.AssetMaintenances
            .Include(m => m.Asset)
            .Where(m => m.NextServiceDue.HasValue && 
                       m.NextServiceDue.Value <= cutoffDate &&
                       m.NextServiceDue.Value >= DateTime.UtcNow)
            .OrderBy(m => m.NextServiceDue)
            .ToListAsync();
    }

    #endregion

    #region Asset Insurance

    public async Task<IEnumerable<AssetInsurance>> GetInsurancePoliciesByAssetAsync(string assetId)
    {
        return await _context.AssetInsurances
            .Where(i => i.AssetId == assetId)
            .OrderByDescending(i => i.PolicyStartDate)
            .ToListAsync();
    }

    public async Task<AssetInsurance> CreateInsurancePolicyAsync(AssetInsurance insurance)
    {
        insurance.Id = Guid.NewGuid().ToString();
        insurance.CreatedAt = DateTime.UtcNow;
        insurance.TouchUpdatedAt();

        _context.AssetInsurances.Add(insurance);
        await _context.SaveChangesAsync();
        return insurance;
    }

    public async Task<AssetInsurance> UpdateInsurancePolicyAsync(AssetInsurance insurance)
    {
        var existing = await _context.AssetInsurances.FindAsync(insurance.Id);
        if (existing == null)
            throw new InvalidOperationException("Insurance policy not found");

        existing.PolicyNumber = insurance.PolicyNumber;
        existing.InsuranceProvider = insurance.InsuranceProvider;
        existing.CoverageAmount = insurance.CoverageAmount;
        existing.AnnualPremium = insurance.AnnualPremium;
        existing.MonthlyPremium = insurance.MonthlyPremium;
        existing.PolicyStartDate = insurance.PolicyStartDate;
        existing.PolicyEndDate = insurance.PolicyEndDate;
        existing.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteInsurancePolicyAsync(string id)
    {
        var insurance = await _context.AssetInsurances.FindAsync(id);
        if (insurance == null)
            return false;

        _context.AssetInsurances.Remove(insurance);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AssetInsurance>> GetExpiringInsurancePoliciesAsync(int daysAhead = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);
        return await _context.AssetInsurances
            .Include(i => i.Asset)
            .Where(i => i.PolicyEndDate <= cutoffDate && i.PolicyEndDate >= DateTime.UtcNow)
            .OrderBy(i => i.PolicyEndDate)
            .ToListAsync();
    }

    #endregion

    #region Asset Documents

    public async Task<IEnumerable<AssetDocument>> GetDocumentsByAssetRegisterAsync(string assetRegisterId)
    {
        return await _context.AssetDocuments
            .Where(d => d.AssetRegisterId == assetRegisterId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<AssetDocument> CreateDocumentAsync(AssetDocument document)
    {
        document.Id = Guid.NewGuid().ToString();
        document.CreatedAt = DateTime.UtcNow;
        document.TouchUpdatedAt();

        _context.AssetDocuments.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<AssetDocument> UpdateDocumentAsync(AssetDocument document)
    {
        var existing = await _context.AssetDocuments.FindAsync(document.Id);
        if (existing == null)
            throw new InvalidOperationException("Document not found");

        existing.DocumentName = document.DocumentName;
        existing.DocumentType = document.DocumentType;
        existing.FilePath = document.FilePath;
        existing.FileSize = document.FileSize;
        existing.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteDocumentAsync(string id)
    {
        var document = await _context.AssetDocuments.FindAsync(id);
        if (document == null)
            return false;

        _context.AssetDocuments.Remove(document);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Depreciation and Valuation

    public async Task<decimal?> CalculateCurrentValueAsync(string assetId)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null)
            return 0;

        return await CalculateDepreciatedValue(asset, DateTime.UtcNow);
    }

    public async Task<decimal?> CalculateDepreciationAsync(string assetId, DateTime? asOfDate = null)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null)
            return 0;

        var currentValue = await CalculateDepreciatedValue(asset, asOfDate ?? DateTime.UtcNow);
        return asset.PurchasePrice - currentValue;
    }

    private async Task<decimal> CalculateDepreciatedValue(Asset asset, DateTime asOfDate)
    {
        if (asset.PurchaseDate > asOfDate)
            return asset.PurchasePrice ?? 0m;

        var yearsElapsed = (decimal)(asOfDate - asset.PurchaseDate.Value).Days / 365.25m;

        return asset.DepreciationMethod?.ToLower() switch
        {
            "straight-line" => CalculateStraightLineDepreciation(asset, yearsElapsed),
            "declining-balance" => CalculateDecliningBalanceDepreciation(asset, yearsElapsed),
            "custom" => asset.CurrentValue ?? 0m, // Use manually set current value
            _ => CalculateStraightLineDepreciation(asset, yearsElapsed) // Default to straight-line
        };
    }

    private decimal CalculateStraightLineDepreciation(Asset asset, decimal yearsElapsed)
    {
        if ((asset.UsefulLifeYears ?? 0) <= 0)
            return asset.PurchasePrice ?? 0m;

        var annualDepreciation = ((asset.PurchasePrice ?? 0m) - (asset.SalvageValue ?? 0m)) / (asset.UsefulLifeYears ?? 1);
        var totalDepreciation = annualDepreciation * yearsElapsed;
        var currentValue = (asset.PurchasePrice ?? 0m) - totalDepreciation;

        return Math.Max(currentValue, asset.SalvageValue ?? 0m);
    }

    private decimal CalculateDecliningBalanceDepreciation(Asset asset, decimal yearsElapsed)
    {
        if ((asset.DepreciationRate ?? 0) <= 0 || (asset.UsefulLifeYears ?? 0) <= 0)
            return asset.PurchasePrice ?? 0m;

        var rate = (asset.DepreciationRate ?? 0m) / 100m;
        var currentValue = (asset.PurchasePrice ?? 0m) * (decimal)Math.Pow((double)(1 - rate), (double)yearsElapsed);

        return Math.Max(currentValue, asset.SalvageValue ?? 0m);
    }

    public async Task<Dictionary<string, decimal?>> GetAssetValuesByCategory()
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.IsActive)
            .GroupBy(a => a.AssetCategory.Name)
            .Select(g => new { Category = g.Key, Value = g.Sum(a => a.CurrentValue) })
            .ToDictionaryAsync(x => x.Category, x => x.Value);
    }

    public async Task<decimal?> GetTotalAssetValueAsync()
    {
        return await _context.Assets
            .Where(a => a.IsActive)
            .SumAsync(a => a.CurrentValue);
    }

    public async Task UpdateAssetValuesAsync()
    {
        var assets = await _context.Assets
            .Where(a => a.IsActive && a.DepreciationMethod != "custom")
            .ToListAsync();

        foreach (var asset in assets)
        {
            asset.CurrentValue = await CalculateDepreciatedValue(asset, DateTime.UtcNow);
            asset.TouchUpdatedAt();
        }

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Reporting and Analytics

    public async Task<Dictionary<string, decimal?>> GetAssetAllocationByCategoryAsync()
    {
        var totalValue = await GetTotalAssetValueAsync();
        if (totalValue == 0)
            return new Dictionary<string, decimal?>();

        var categoryValues = await GetAssetValuesByCategory();
        return categoryValues.ToDictionary(
            kvp => kvp.Key,
            kvp => (decimal?)Math.Round((kvp.Value ?? 0.001m / totalValue?? 1) * 100, 2)
        );
    }

    public async Task<Dictionary<string, int>> GetAssetCountByCategoryAsync()
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.IsActive)
            .GroupBy(a => a.AssetCategory.Name)
            .Select(g => new { Category = g.Key, Value = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Value);
    }

    public async Task<IEnumerable<Asset>> GetTopAssetsByValueAsync(int count = 10)
    {
        return await _context.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CurrentValue)
            .Take(count)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalDepreciationAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Assets.Where(a => a.IsActive);

        if (startDate.HasValue)
            query = query.Where(a => a.PurchaseDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.PurchaseDate <= endDate.Value);

        var assets = await query.ToListAsync();
        decimal totalDepreciation = 0;

        foreach (var asset in assets)
        {
            totalDepreciation += (await CalculateDepreciationAsync(asset.Id, endDate)) ?? 0;
        }

        return totalDepreciation;
    }

    public async Task<Dictionary<DateTime, decimal?>> GetAssetValueTrendAsync(int months = 12)
    {
        var result = new Dictionary<DateTime, decimal?>();
        var startDate = DateTime.UtcNow.AddMonths(-months);

        for (int i = 0; i <= months; i++)
        {
            var date = startDate.AddMonths(i);
            var monthEnd = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            
            var assets = await _context.Assets
                .Where(a => a.PurchaseDate <= monthEnd)
                .ToListAsync();

            decimal? totalValue = null;
            foreach (var asset in assets)
            {
                totalValue += await CalculateDepreciatedValue(asset, monthEnd);
            }

            result[monthEnd] = totalValue;
        }

        return result;
    }

    #endregion

    #region Asset Linking

    public async Task<bool> LinkAssetToTransactionAsync(string assetId, string transactionId)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        var transaction = await _context.Transactions.FindAsync(transactionId);

        if (asset == null || transaction == null)
            return false;

        // Update transaction with asset reference (assuming there's an AssetId field)
        // This would require adding AssetId to Transaction model
        // transaction.AssetId = assetId;
        // await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnlinkAssetFromTransactionAsync(string assetId, string transactionId)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction == null)
            return false;

        // Remove asset reference from transaction
        // transaction.AssetId = null;
        // await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Transaction>> GetAssetTransactionsAsync(string assetId)
    {
        // This would require adding AssetId to Transaction model
        return await _context.Transactions
            .Where(t => t.Description.Contains(assetId)) // Temporary implementation
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    #endregion

    #region Validation and Business Rules

    public async Task<bool> ValidateAssetAsync(Asset asset)
    {
        if (string.IsNullOrWhiteSpace(asset.Name) || asset.Name.Length > 200)
            return false;

        if (asset.PurchasePrice < 0)
            return false;

        if (asset.CurrentValue < 0)
            return false;

        if (asset.SalvageValue < 0 || asset.SalvageValue > asset.PurchasePrice)
            return false;

        if (asset.UsefulLifeYears < 0)
            return false;

        if (asset.DepreciationRate < 0 || asset.DepreciationRate > 100)
            return false;

        // Validate category exists
        if (!string.IsNullOrEmpty(asset.AssetCategoryId))
        {
            var categoryExists = await _context.AssetCategories
                .AnyAsync(c => c.Id == asset.AssetCategoryId && c.IsActive);
            if (!categoryExists)
                return false;
        }

        return true;
    }

    public async Task<bool> CanDeleteAssetAsync(string assetId)
    {
        // Check if asset has active insurance policies
        var hasActiveInsurance = await _context.AssetInsurances
            .AnyAsync(i => i.AssetId == assetId && i.PolicyEndDate > DateTime.UtcNow);

        // Check if asset has recent maintenance records
        var hasRecentMaintenance = await _context.AssetMaintenances
            .AnyAsync(m => m.AssetId == assetId && m.ServiceDate > DateTime.UtcNow.AddMonths(-6));

        // Allow deletion but warn user about these conditions
        return true;
    }

    public async Task<bool> IsSerialNumberUniqueAsync(string serialNumber, string? excludeAssetId = null)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return true;

        var query = _context.Assets.Where(a => a.SerialNumber == serialNumber && a.IsActive);
        
        if (!string.IsNullOrEmpty(excludeAssetId))
            query = query.Where(a => a.Id != excludeAssetId);

        return !await query.AnyAsync();
    }

    #endregion

    #region Bulk Operations

    public async Task<bool> BulkUpdateAssetValuesAsync(Dictionary<string, decimal?> assetValues)
    {
        var assetIds = assetValues.Keys.ToList();
        var assets = await _context.Assets
            .Where(a => assetIds.Contains(a.Id))
            .ToListAsync();

        foreach (var asset in assets)
        {
            if (assetValues.TryGetValue(asset.Id, out var newValue))
            {
                asset.CurrentValue = newValue;
                asset.TouchUpdatedAt();
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BulkUpdateDepreciationAsync(IEnumerable<string> assetIds)
    {
        var assets = await _context.Assets
            .Where(a => assetIds.Contains(a.Id) && a.IsActive)
            .ToListAsync();

        foreach (var asset in assets)
        {
            if (asset.DepreciationMethod != "custom")
            {
                asset.CurrentValue = await CalculateDepreciatedValue(asset, DateTime.UtcNow);
                asset.TouchUpdatedAt();
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Asset>> ImportAssetsAsync(IEnumerable<Asset> assets)
    {
        var importedAssets = new List<Asset>();

        foreach (var asset in assets)
        {
            try
            {
                if (await ValidateAssetAsync(asset))
                {
                    var createdAsset = await CreateAssetAsync(asset);
                    importedAssets.Add(createdAsset);
                }
            }
            catch (Exception)
            {
                // Log error and continue with next asset
                continue;
            }
        }

        return importedAssets;
    }

    #endregion
}