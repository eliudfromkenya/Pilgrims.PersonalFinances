using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Date range presets for reports
    /// </summary>
    public enum DateRangePreset
    {
        [Display(Name = "Today")]
        Today,
        
        [Display(Name = "Yesterday")]
        Yesterday,
        
        [Display(Name = "This Week")]
        ThisWeek,
        
        [Display(Name = "Last Week")]
        LastWeek,
        
        [Display(Name = "This Month")]
        ThisMonth,
        
        [Display(Name = "Last Month")]
        LastMonth,
        
        [Display(Name = "This Quarter")]
        ThisQuarter,
        
        [Display(Name = "Last Quarter")]
        LastQuarter,
        
        [Display(Name = "This Year")]
        ThisYear,
        
        [Display(Name = "Last Year")]
        LastYear,
        
        [Display(Name = "Year to Date")]
        YearToDate,
        
        [Display(Name = "Last 30 Days")]
        Last30Days,
        
        [Display(Name = "Last 90 Days")]
        Last90Days,
        
        [Display(Name = "Last 12 Months")]
        Last12Months,
        
        [Display(Name = "Custom Range")]
        CustomRange
    }
}
