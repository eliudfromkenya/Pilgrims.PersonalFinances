using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    /// <summary>
    /// Data transfer object for investment reports
    /// </summary>
    public class InvestmentReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? PortfolioId { get; set; }
        public string? PortfolioName { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalValue { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalGainLoss { get; set; }
        
        public decimal TotalReturnPercentage { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DividendIncome { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal RealizedGains { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnrealizedGains { get; set; }
        
        public List<InvestmentPerformanceDto> InvestmentPerformance { get; set; } = new();
        public PortfolioAllocationDto PortfolioAllocation { get; set; } = new();
    }

    /// <summary>
    /// Individual investment performance
    /// </summary>
    public class InvestmentPerformanceDto
    {
        public string? InvestmentId { get; set; }
        public string InvestmentName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal AverageCost { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentPrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal MarketValue { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal GainLoss { get; set; }
        
        public decimal ReturnPercentage { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DividendYield { get; set; }
        
        public decimal AllocationPercentage { get; set; }
    }

    /// <summary>
    /// Portfolio allocation breakdown
    /// </summary>
    public class PortfolioAllocationDto
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalValue { get; set; }
        
        public List<AllocationByAssetType> AssetTypeAllocation { get; set; } = new();
        public List<AllocationBySector> SectorAllocation { get; set; } = new();
        public List<AllocationByRegion> RegionAllocation { get; set; } = new();
    }

    /// <summary>
    /// Allocation by asset type
    /// </summary>
    public class AllocationByAssetType
    {
        public string AssetType { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }
        
        public decimal Percentage { get; set; }
        public int InvestmentCount { get; set; }
    }

    /// <summary>
    /// Allocation by sector
    /// </summary>
    public class AllocationBySector
    {
        public string Sector { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }
        
        public decimal Percentage { get; set; }
        public int InvestmentCount { get; set; }
    }

    /// <summary>
    /// Allocation by region
    /// </summary>
    public class AllocationByRegion
    {
        public string Region { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }
        
        public decimal Percentage { get; set; }
        public int InvestmentCount { get; set; }
    }
}
