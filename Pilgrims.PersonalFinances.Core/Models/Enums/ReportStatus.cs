using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Report generation status
    /// </summary>
    public enum ReportStatus
    {
        [Display(Name = "Pending")]
        Pending,
        
        [Display(Name = "Processing")]
        Processing,
        
        [Display(Name = "Generating")]
        Generating,
        
        [Display(Name = "Completed")]
        Completed,
        
        [Display(Name = "Failed")]
        Failed,
        
        [Display(Name = "Cancelled")]
        Cancelled
    }
}