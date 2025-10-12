using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    public class CategorySummaryDto
    {
        public string CategoryId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }
}

