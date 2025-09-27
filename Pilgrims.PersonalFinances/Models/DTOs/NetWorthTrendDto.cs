using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
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
        
        // Additional properties for compatibility with NetWorthTrendReport.razor
        public decimal CurrentNetWorth 
        { 
            get { return EndingNetWorth; } 
        }
        
        public decimal NetWorthChange 
        { 
            get { return Change; } 
        }
        
        public decimal GrowthRate 
        { 
            get { return ChangePercentage; } 
        }
        
        public List<NetWorthDataPoint> MonthlyNetWorth 
        { 
            get { return DataPoints; } 
        }
    }

    /// <summary>
    /// Individual data point for net worth trends
    /// </summary>
    public class NetWorthDataPoint
    {
        public DateTime Date { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetWorth { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Assets { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Liabilities { get; set; }
    }
}