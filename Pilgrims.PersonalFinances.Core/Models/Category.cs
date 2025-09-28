using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Models;

/// <summary>
/// Represents a hierarchical category for organizing transactions
/// </summary>
public class Category : BaseEntity
{
    private string _name = string.Empty;
    private string? _description;
    private string _colorCode = "#6366F1";
    private string? _iconName;
    private bool _isActive = true;
    private string? _parentCategoryId;
    private Category? _parentCategory;
    private List<Category> _subCategories = new();
    private List<Transaction> _transactions = new();

    /// <summary>
    /// Category name (required, max 100 characters)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Optional description of the category (max 500 characters)
    /// </summary>
    [StringLength(500)]
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// Color code for visual representation (hex format)
    /// </summary>
    [Required]
    [StringLength(7)]
    public string ColorCode
    {
        get => _colorCode;
        set => SetProperty(ref _colorCode, value);
    }

    /// <summary>
    /// Optional icon name for visual representation
    /// </summary>
    [StringLength(50)]
    public string? IconName
    {
        get => _iconName;
        set => SetProperty(ref _iconName, value);
    }

    /// <summary>
    /// Whether this category is active and available for use
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    /// <summary>
    /// ID of the parent category (null for root categories)
    /// </summary>
    public string? ParentCategoryId
    {
        get => _parentCategoryId;
        set => SetProperty(ref _parentCategoryId, value);
    }

    /// <summary>
    /// Navigation property to parent category
    /// </summary>
    public Category? ParentCategory
    {
        get => _parentCategory;
        set => SetProperty(ref _parentCategory, value);
    }

    /// <summary>
    /// Navigation property to child categories
    /// </summary>
    public List<Category> SubCategories
    {
        get => _subCategories;
        set => SetProperty(ref _subCategories, value);
    }

    /// <summary>
    /// Navigation property to transactions using this category
    /// </summary>
    public List<Transaction> Transactions
    {
        get => _transactions;
        set => SetProperty(ref _transactions, value);
    }

    /// <summary>
    /// ID of the user who owns this category
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets the full hierarchical path of the category (e.g., "Food > Restaurants")
    /// </summary>
    public string FullPath
    {
        get
        {
            if (ParentCategory == null)
                return Name;
            
            return $"{ParentCategory.FullPath} > {Name}";
        }
    }

    /// <summary>
    /// Gets whether this is a root category (has no parent)
    /// </summary>
    public bool IsRootCategory => string.IsNullOrEmpty(ParentCategoryId);

    /// <summary>
    /// Gets whether this category has child categories
    /// </summary>
    public bool HasSubCategories => SubCategories.Any();

    /// <summary>
    /// Gets the depth level of this category in the hierarchy (0 for root)
    /// </summary>
    public int Level
    {
        get
        {
            if (ParentCategory == null)
                return 0;
            
            return ParentCategory.Level + 1;
        }
    }

    /// <summary>
    /// Validates the category data
    /// </summary>
    /// <returns>List of validation errors</returns>
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Category name is required");

        if (Name?.Length > 100)
            errors.Add("Category name cannot exceed 100 characters");

        if (Description?.Length > 500)
            errors.Add("Description cannot exceed 500 characters");

        if (!IsValidColorCode(ColorCode))
            errors.Add("Color code must be a valid hex color (e.g., #FF0000)");

        if (IconName?.Length > 50)
            errors.Add("Icon name cannot exceed 50 characters");

        // Prevent circular references
        if (ParentCategoryId == Id)
            errors.Add("Category cannot be its own parent");

        return errors;
    }

    /// <summary>
    /// Validates if the provided color code is a valid hex color
    /// </summary>
    /// <param name="colorCode">Color code to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool IsValidColorCode(string colorCode)
    {
        if (string.IsNullOrWhiteSpace(colorCode))
            return false;

        if (!colorCode.StartsWith("#"))
            return false;

        if (colorCode.Length != 7)
            return false;

        return colorCode[1..].All(c => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
    }

    /// <summary>
    /// Gets all descendant categories (children, grandchildren, etc.)
    /// </summary>
    /// <returns>Flattened list of all descendant categories</returns>
    public List<Category> GetAllDescendants()
    {
        var descendants = new List<Category>();
        
        foreach (var subCategory in SubCategories)
        {
            descendants.Add(subCategory);
            descendants.AddRange(subCategory.GetAllDescendants());
        }
        
        return descendants;
    }

    /// <summary>
    /// Gets all ancestor categories (parent, grandparent, etc.)
    /// </summary>
    /// <returns>List of ancestor categories from immediate parent to root</returns>
    public List<Category> GetAllAncestors()
    {
        var ancestors = new List<Category>();
        var current = ParentCategory;
        
        while (current != null)
        {
            ancestors.Add(current);
            current = current.ParentCategory;
        }
        
        return ancestors;
    }
}