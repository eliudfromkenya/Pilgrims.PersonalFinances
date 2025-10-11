namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    public class ReconciliationValidationResult
    {
        public bool IsValid { get; set; }
        public decimal CalculatedDifference { get; set; }
        public decimal ReportedDifference { get; set; }
        public IEnumerable<string> ValidationErrors { get; set; } = new List<string>();
        public IEnumerable<string> Warnings { get; set; } = new List<string>();
    }
}
