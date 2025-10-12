namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    public class CategoryVarianceDto
    {
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal BudgetedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
