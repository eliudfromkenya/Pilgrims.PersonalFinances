using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

/// <summary>
/// Service interface for comprehensive asset management with CRUD operations, depreciation tracking, and reporting
/// </summary>
public interface IAssetService
{
    // Basic CRUD Operations - Assets
    Task<IEnumerable<Asset>> GetAllAssetsAsync();
    Task<Asset?> GetAssetByIdAsync(string id);
    Task<Asset> CreateAssetAsync(Asset asset);
    Task<Asset> UpdateAssetAsync(Asset asset);
    Task<bool> DeleteAssetAsync(string id);
    Task<bool> DeleteAssetsAsync(IEnumerable<string> ids);

    // Asset Filtering and Search
    Task<IEnumerable<Asset>> GetAssetsByCategoryAsync(string categoryId);
    Task<IEnumerable<Asset>> GetActiveAssetsAsync();
    Task<IEnumerable<Asset>> GetDisposedAssetsAsync();
    Task<IEnumerable<Asset>> SearchAssetsAsync(string searchTerm);
    Task<IEnumerable<Asset>> GetAssetsByLocationAsync(string location);
    Task<IEnumerable<Asset>> GetAssetsByConditionAsync(string condition);
    Task<IEnumerable<Asset>> GetAssetsByDateRangeAsync(DateTime startDate, DateTime endDate);

    // Asset Categories
    Task<IEnumerable<AssetCategory>> GetAllAssetCategoriesAsync();
    Task<AssetCategory?> GetAssetCategoryByIdAsync(string id);
    Task<AssetCategory> CreateAssetCategoryAsync(AssetCategory category);
    Task<AssetCategory> UpdateAssetCategoryAsync(AssetCategory category);
    Task<bool> DeleteAssetCategoryAsync(string id);

    // Asset Register Operations
    Task<AssetRegister?> GetAssetRegisterByAssetIdAsync(string assetId);
    Task<AssetRegister> CreateOrUpdateAssetRegisterAsync(AssetRegister assetRegister);
    Task<bool> DeleteAssetRegisterAsync(string id);

    // Asset Maintenance
    Task<IEnumerable<AssetMaintenance>> GetMaintenanceRecordsByAssetAsync(string assetId);
    Task<AssetMaintenance> CreateMaintenanceRecordAsync(AssetMaintenance maintenance);
    Task<AssetMaintenance> UpdateMaintenanceRecordAsync(AssetMaintenance maintenance);
    Task<bool> DeleteMaintenanceRecordAsync(string id);
    Task<IEnumerable<AssetMaintenance>> GetUpcomingMaintenanceAsync(int daysAhead = 30);

    // Asset Insurance
    Task<IEnumerable<AssetInsurance>> GetInsurancePoliciesByAssetAsync(string assetId);
    Task<AssetInsurance> CreateInsurancePolicyAsync(AssetInsurance insurance);
    Task<AssetInsurance> UpdateInsurancePolicyAsync(AssetInsurance insurance);
    Task<bool> DeleteInsurancePolicyAsync(string id);
    Task<IEnumerable<AssetInsurance>> GetExpiringInsurancePoliciesAsync(int daysAhead = 30);

    // Asset Documents
    Task<IEnumerable<AssetDocument>> GetDocumentsByAssetRegisterAsync(string assetRegisterId);
    Task<AssetDocument> CreateDocumentAsync(AssetDocument document);
    Task<AssetDocument> UpdateDocumentAsync(AssetDocument document);
    Task<bool> DeleteDocumentAsync(string id);

    // Depreciation and Valuation
    Task<decimal?> CalculateCurrentValueAsync(string assetId);
    Task<decimal?> CalculateDepreciationAsync(string assetId, DateTime? asOfDate = null);
    Task<Dictionary<string, decimal?>> GetAssetValuesByCategory();
    Task<decimal?> GetTotalAssetValueAsync();
    Task UpdateAssetValuesAsync();

    // Reporting and Analytics
    Task<Dictionary<string, decimal?>> GetAssetAllocationByCategoryAsync();
    Task<Dictionary<string, int>> GetAssetCountByCategoryAsync();
    Task<IEnumerable<Asset>> GetTopAssetsByValueAsync(int count = 10);
    Task<decimal> GetTotalDepreciationAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<Dictionary<DateTime, decimal?>> GetAssetValueTrendAsync(int months = 12);

    // Asset Linking
    Task<bool> LinkAssetToTransactionAsync(string assetId, string transactionId);
    Task<bool> UnlinkAssetFromTransactionAsync(string assetId, string transactionId);
    Task<IEnumerable<Transaction>> GetAssetTransactionsAsync(string assetId);

    // Validation and Business Rules
    Task<bool> ValidateAssetAsync(Asset asset);
    Task<bool> CanDeleteAssetAsync(string assetId);
    Task<bool> IsSerialNumberUniqueAsync(string serialNumber, string? excludeAssetId = null);

    // Bulk Operations
    Task<bool> BulkUpdateAssetValuesAsync(Dictionary<string, decimal?> assetValues);
    Task<bool> BulkUpdateDepreciationAsync(IEnumerable<string> assetIds);
    Task<IEnumerable<Asset>> ImportAssetsAsync(IEnumerable<Asset> assets);
}
