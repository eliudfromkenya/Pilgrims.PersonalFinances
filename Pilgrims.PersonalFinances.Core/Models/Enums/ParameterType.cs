using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Report parameter data types
    /// </summary>
    public enum ParameterType
    {
        [Display(Name = "Text")]
        Text,
        
        [Display(Name = "Number")]
        Number,
        
        [Display(Name = "Date")]
        Date,
        
        [Display(Name = "Date Range")]
        DateRange,
        
        [Display(Name = "Boolean")]
        Boolean,
        
        [Display(Name = "Dropdown")]
        Dropdown,
        
        [Display(Name = "Multi-Select")]
        MultiSelect,
        
        [Display(Name = "Account")]
        Account,
        
        [Display(Name = "Category")]
        Category,
        
        [Display(Name = "Budget")]
        Budget
    }
}
