using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Data transfer object for balance sheet reports
    /// </summary>
    public class BalanceSheetDto
    {
        public DateTime AsOfDate { get; set; }
        
        // Assets
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAssets { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentAssets { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal FixedAssets { get; set; }
        
        public List<AssetLiabilityDto> AssetBreakdown { get; set; } = new();
        
        // Liabilities
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalLiabilities { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentLiabilities { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal LongTermLiabilities { get; set; }
        
        public List<AssetLiabilityDto> LiabilityBreakdown { get; set; } = new();
        
        // Net Worth
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetWorth { get; set; }
    }

    /// <summary>
    /// Asset or liability breakdown for balance sheet
    /// </summary>
    public class AssetLiabilityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public decimal Percentage { get; set; }
        public bool IsCurrent { get; set; }
    }

    /// <summary>
    /// Net worth trend data
    /// </summary>
    public class NetWorthTrendDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal StartingNetWorth { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal EndingNetWorth { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Change { get; set; }
        
        public decimal ChangePercentage { get; set; }
        
        public List<NetWorthDataPoint> DataPoints { get; set; } = new();
    }

    /// <summary>
    /// Individual data point for net worth trends
    /// </summary>
    public class NetWorthDataPoint
    {
        public DateTime Date { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Assets { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Liabilities { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetWorth { get; set; }
    }
}