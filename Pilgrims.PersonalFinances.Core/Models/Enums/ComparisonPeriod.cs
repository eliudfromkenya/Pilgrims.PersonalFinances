using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Comparison period types for reports
    /// </summary>
    public enum ComparisonPeriod
    {
        [Display(Name = "None")]
        None,
        
        [Display(Name = "Previous Period")]
        PreviousPeriod,
        
        [Display(Name = "Same Period Last Year")]
        SamePeriodLastYear,
        
        [Display(Name = "Year over Year")]
        YearOverYear,
        
        [Display(Name = "Month over Month")]
        MonthOverMonth,
        
        [Display(Name = "Quarter over Quarter")]
        QuarterOverQuarter
    }
}