using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models
{
    public class AssetCategory : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; } // Icon name for UI display

        [MaxLength(20)]
        public string? Color { get; set; } // Color code for UI display

        // Default depreciation settings for this category
        [MaxLength(50)]
        public string? DefaultDepreciationMethod { get; set; }

        public decimal? DefaultDepreciationRate { get; set; }

        public int? DefaultUsefulLifeYears { get; set; }

        // Category status
        public bool IsActive { get; set; } = true;

        // Audit fields - CreatedAt and UpdatedAt are inherited from BaseEntity

        // Navigation properties
        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();

        // Static method to get default categories
        public static List<AssetCategory> GetDefaultCategories()
        {
            return new List<AssetCategory>
            {
                new AssetCategory
                {
                    Name = "Vehicle",
                    Description = "Cars, motorcycles, boats, and other vehicles",
                    Icon = "car",
                    Color = "#FF6B6B",
                    DefaultDepreciationMethod = "Declining balance",
                    DefaultDepreciationRate = 20,
                    DefaultUsefulLifeYears = 10
                },
                new AssetCategory
                {
                    Name = "Electronics",
                    Description = "Computers, phones, TVs, and electronic devices",
                    Icon = "laptop",
                    Color = "#4ECDC4",
                    DefaultDepreciationMethod = "Declining balance",
                    DefaultDepreciationRate = 25,
                    DefaultUsefulLifeYears = 5
                },
                new AssetCategory
                {
                    Name = "Furniture",
                    Description = "Home and office furniture",
                    Icon = "home",
                    Color = "#45B7D1",
                    DefaultDepreciationMethod = "Straight-line",
                    DefaultDepreciationRate = 10,
                    DefaultUsefulLifeYears = 15
                },
                new AssetCategory
                {
                    Name = "Jewelry",
                    Description = "Jewelry, watches, and precious items",
                    Icon = "diamond",
                    Color = "#F7DC6F",
                    DefaultDepreciationMethod = "Custom",
                    DefaultDepreciationRate = 0,
                    DefaultUsefulLifeYears = null
                },
                new AssetCategory
                {
                    Name = "Appliances",
                    Description = "Kitchen and household appliances",
                    Icon = "kitchen",
                    Color = "#BB8FCE",
                    DefaultDepreciationMethod = "Straight-line",
                    DefaultDepreciationRate = 15,
                    DefaultUsefulLifeYears = 10
                },
                new AssetCategory
                {
                    Name = "Tools & Equipment",
                    Description = "Tools, machinery, and equipment",
                    Icon = "tools",
                    Color = "#85C1E9",
                    DefaultDepreciationMethod = "Straight-line",
                    DefaultDepreciationRate = 12,
                    DefaultUsefulLifeYears = 12
                },
                new AssetCategory
                {
                    Name = "Art & Collectibles",
                    Description = "Artwork, collectibles, and antiques",
                    Icon = "palette",
                    Color = "#F8C471",
                    DefaultDepreciationMethod = "Custom",
                    DefaultDepreciationRate = 0,
                    DefaultUsefulLifeYears = null
                },
                new AssetCategory
                {
                    Name = "Real Estate",
                    Description = "Property and real estate investments",
                    Icon = "building",
                    Color = "#82E0AA",
                    DefaultDepreciationMethod = "Straight-line",
                    DefaultDepreciationRate = 3,
                    DefaultUsefulLifeYears = 30
                },
                new AssetCategory
                {
                    Name = "Sports & Recreation",
                    Description = "Sports equipment, recreational vehicles, and hobby items",
                    Icon = "sports",
                    Color = "#F1948A",
                    DefaultDepreciationMethod = "Declining balance",
                    DefaultDepreciationRate = 15,
                    DefaultUsefulLifeYears = 8
                },
                new AssetCategory
                {
                    Name = "Other",
                    Description = "Miscellaneous assets not fitting other categories",
                    Icon = "more",
                    Color = "#D5DBDB",
                    DefaultDepreciationMethod = "Straight-line",
                    DefaultDepreciationRate = 10,
                    DefaultUsefulLifeYears = 10
                }
            };
        }
    }
}
