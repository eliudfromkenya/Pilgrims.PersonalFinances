namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents the result of a validation operation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Gets or sets whether the validation was successful
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the list of validation errors
        /// </summary>
        public List<string> Errors { get; set; } = [];

        /// <summary>
        /// Creates a successful validation result
        /// </summary>
        /// <returns>A valid ValidationResult</returns>
        public static ValidationResult Success()
        {
            return new ValidationResult { IsValid = true };
        }

        /// <summary>
        /// Creates a failed validation result with errors
        /// </summary>
        /// <param name="errors">List of validation errors</param>
        /// <returns>An invalid ValidationResult</returns>
        public static ValidationResult Failure(List<string> errors)
        {
            return new ValidationResult { IsValid = false, Errors = errors };
        }

        /// <summary>
        /// Creates a failed validation result with a single error
        /// </summary>
        /// <param name="error">Single validation error</param>
        /// <returns>An invalid ValidationResult</returns>
        public static ValidationResult Failure(string error)
        {
            return new ValidationResult { IsValid = false, Errors = new List<string> { error } };
        }
        public void AddError(string str)
        {
            Errors?.Add(str);    
        }
    }
}