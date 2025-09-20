using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    public class ReportParameter : BaseEntity
    {
        /// <summary>
        /// Reference to the report template
        /// </summary>
        [Required]
        public int ReportTemplateId { get; set; }

        /// <summary>
        /// Parameter name/key
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Display label for the parameter
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Parameter description or help text
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Type of parameter (string, number, date, boolean, etc.)
        /// </summary>
        [Required]
        public ParameterType ParameterType { get; set; }

        /// <summary>
        /// Default value for the parameter
        /// </summary>
        [StringLength(500)]
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Whether this parameter is required
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// Whether this parameter allows multiple values
        /// </summary>
        public bool AllowMultiple { get; set; } = false;

        /// <summary>
        /// Display order for parameter listing
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Validation rules (JSON format)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? ValidationRules { get; set; }

        /// <summary>
        /// Available options for dropdown/select parameters (JSON format)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Options { get; set; }

        /// <summary>
        /// Minimum value for numeric parameters
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Maximum value for numeric parameters
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// Minimum date for date parameters
        /// </summary>
        public DateTime? MinDate { get; set; }

        /// <summary>
        /// Maximum date for date parameters
        /// </summary>
        public DateTime? MaxDate { get; set; }

        /// <summary>
        /// Regular expression pattern for string validation
        /// </summary>
        [StringLength(200)]
        public string? Pattern { get; set; }

        /// <summary>
        /// Minimum length for string parameters
        /// </summary>
        public int? MinLength { get; set; }

        /// <summary>
        /// Maximum length for string parameters
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Validation pattern for string parameters
        /// </summary>
        [StringLength(200)]
        public string? ValidationPattern { get; set; }

        /// <summary>
        /// Placeholder text for input fields
        /// </summary>
        [StringLength(100)]
        public string? Placeholder { get; set; }

        /// <summary>
        /// CSS class for styling the parameter input
        /// </summary>
        [StringLength(100)]
        public string? CssClass { get; set; }

        /// <summary>
        /// Whether this parameter is visible to users
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Whether this parameter is read-only
        /// </summary>
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// Group or section for organizing parameters
        /// </summary>
        [StringLength(50)]
        public string? Group { get; set; }

        /// <summary>
        /// Conditional visibility rules (JSON format)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? ConditionalRules { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(ReportTemplateId))]
        public virtual ReportTemplate ReportTemplate { get; set; } = null!;

        // Computed Properties
        [NotMapped]
        public bool IsNumeric => ParameterType == ParameterType.Number;

        [NotMapped]
        public bool IsDate => ParameterType == ParameterType.Date || ParameterType == ParameterType.DateRange;

        [NotMapped]
        public bool IsText => ParameterType == ParameterType.Text;

        [NotMapped]
        public bool IsSelection => ParameterType == ParameterType.Dropdown || ParameterType == ParameterType.MultiSelect;

        [NotMapped]
        public List<string> OptionsList
        {
            get
            {
                if (string.IsNullOrEmpty(Options))
                    return new List<string>();

                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<string>>(Options) ?? new List<string>();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }

        // Methods
        public bool ValidateValue(string? value)
        {
            if (IsRequired && string.IsNullOrEmpty(value))
                return false;

            if (string.IsNullOrEmpty(value))
                return true; // Optional parameter with no value is valid

            switch (ParameterType)
            {
                case ParameterType.Number:
                    if (!int.TryParse(value, out int intValue))
                        return false;
                    if (MinValue.HasValue && intValue < MinValue.Value)
                        return false;
                    if (MaxValue.HasValue && intValue > MaxValue.Value)
                        return false;
                    break;

                case ParameterType.Date:
                case ParameterType.DateRange:
                    if (!DateTime.TryParse(value, out DateTime dateValue))
                        return false;
                    if (MinDate.HasValue && dateValue < MinDate.Value)
                        return false;
                    if (MaxDate.HasValue && dateValue > MaxDate.Value)
                        return false;
                    break;

                case ParameterType.Boolean:
                    if (!bool.TryParse(value, out _))
                        return false;
                    break;

                case ParameterType.Text:
                    if (MinLength.HasValue && value.Length < MinLength.Value)
                        return false;
                    if (MaxLength.HasValue && value.Length > MaxLength.Value)
                        return false;
                    break;

                case ParameterType.Dropdown:
                    if (!string.IsNullOrEmpty(ValidationPattern) && !System.Text.RegularExpressions.Regex.IsMatch(value, ValidationPattern))
                        return false;
                    break;

                case ParameterType.MultiSelect:
                    var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (MinValue.HasValue && values.Length < MinValue.Value)
                        return false;
                    if (MaxValue.HasValue && values.Length > MaxValue.Value)
                        return false;
                    break;
            }

            return true;
        }

        public object? ParseValue(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return ParameterType switch
            {
                ParameterType.Number => int.TryParse(value, out int intVal) ? intVal : null,
                ParameterType.Date => DateTime.TryParse(value, out DateTime dateVal) ? dateVal.Date : null,
                ParameterType.DateRange => DateTime.TryParse(value, out DateTime dtVal) ? dtVal : null,
                ParameterType.Boolean => bool.TryParse(value, out bool boolVal) ? boolVal : null,
                ParameterType.MultiSelect => value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToArray(),
                _ => value
            };
        }

        public void SetOptions(List<string> options)
        {
            Options = System.Text.Json.JsonSerializer.Serialize(options);
            MarkAsDirty();
        }

        public void SetValidationRules(object rules)
        {
            ValidationRules = System.Text.Json.JsonSerializer.Serialize(rules);
            MarkAsDirty();
        }

        public void SetConditionalRules(object rules)
        {
            ConditionalRules = System.Text.Json.JsonSerializer.Serialize(rules);
            MarkAsDirty();
        }
    }
}