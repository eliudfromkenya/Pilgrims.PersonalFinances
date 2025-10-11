using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Export formats available for reports
    /// </summary>
    public enum ExportFormat
    {
        [Display(Name = "PDF")]
        PDF,
        
        [Display(Name = "CSV")]
        CSV,
        
        [Display(Name = "Excel")]
        Excel,
        
        [Display(Name = "PNG Image")]
        PNG,
        
        [Display(Name = "JPEG Image")]
        JPEG,
        
        [Display(Name = "SVG")]
        SVG,
        
        [Display(Name = "JSON")]
        JSON,
        
        [Display(Name = "XML")]
        XML
    }
}
