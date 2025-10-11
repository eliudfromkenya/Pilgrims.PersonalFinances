using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Report frequency for scheduled reports
    /// </summary>
    public enum ReportFrequency
    {
        [Display(Name = "One Time")]
        OneTime,
        
        [Display(Name = "Daily")]
        Daily,
        
        [Display(Name = "Weekly")]
        Weekly,
        
        [Display(Name = "Monthly")]
        Monthly,
        
        [Display(Name = "Quarterly")]
        Quarterly,
        
        [Display(Name = "Yearly")]
        Yearly
    }
}
