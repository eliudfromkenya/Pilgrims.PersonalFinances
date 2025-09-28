using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a user in the personal finance system
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// User's email address (used as username)
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's first name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password
        /// </summary>
        [Required]
        [StringLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Salt used for password hashing
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PasswordSalt { get; set; } = string.Empty;

        /// <summary>
        /// Phone number (optional)
        /// </summary>
        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// User's date of birth (optional)
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// User's preferred currency
        /// </summary>
        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// User's timezone
        /// </summary>
        [StringLength(50)]
        public string TimeZone { get; set; } = "UTC";

        /// <summary>
        /// Whether the user's email is verified
        /// </summary>
        public bool IsEmailVerified { get; set; } = false;

        /// <summary>
        /// Whether the user account is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Whether the user account is locked
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Number of failed login attempts
        /// </summary>
        public int FailedLoginAttempts { get; set; } = 0;

        /// <summary>
        /// When the account was locked (if applicable)
        /// </summary>
        public DateTime? LockedAt { get; set; }

        /// <summary>
        /// Last successful login timestamp
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Email verification token
        /// </summary>
        [StringLength(500)]
        public string? EmailVerificationToken { get; set; }

        /// <summary>
        /// When the email verification token expires
        /// </summary>
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        /// <summary>
        /// Password reset token
        /// </summary>
        [StringLength(500)]
        public string? PasswordResetToken { get; set; }

        /// <summary>
        /// When the password reset token expires
        /// </summary>
        public DateTime? PasswordResetTokenExpiry { get; set; }

        /// <summary>
        /// User's profile picture path (optional)
        /// </summary>
        [StringLength(500)]
        public string? ProfilePicturePath { get; set; }

        /// <summary>
        /// User preferences (JSON format)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Preferences { get; set; }

        // Navigation Properties
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        // Computed Properties
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [NotMapped]
        public string DisplayName => !string.IsNullOrEmpty(FirstName) ? FirstName : Email;

        [NotMapped]
        public bool IsEmailVerificationTokenValid => 
            !string.IsNullOrEmpty(EmailVerificationToken) && 
            EmailVerificationTokenExpiry.HasValue && 
            EmailVerificationTokenExpiry.Value > DateTime.UtcNow;

        [NotMapped]
        public bool IsPasswordResetTokenValid => 
            !string.IsNullOrEmpty(PasswordResetToken) && 
            PasswordResetTokenExpiry.HasValue && 
            PasswordResetTokenExpiry.Value > DateTime.UtcNow;

        [NotMapped]
        public bool CanLogin => IsActive && !IsLocked && IsEmailVerified;

        // Methods
        public void MarkEmailAsVerified()
        {
            IsEmailVerified = true;
            EmailVerificationToken = null;
            EmailVerificationTokenExpiry = null;
            MarkAsDirty();
        }

        public void GenerateEmailVerificationToken()
        {
            EmailVerificationToken = Guid.NewGuid().ToString("N");
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
            MarkAsDirty();
        }

        public void GeneratePasswordResetToken()
        {
            PasswordResetToken = Guid.NewGuid().ToString("N");
            PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(2);
            MarkAsDirty();
        }

        public void ClearPasswordResetToken()
        {
            PasswordResetToken = null;
            PasswordResetTokenExpiry = null;
            MarkAsDirty();
        }

        public void RecordSuccessfulLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            FailedLoginAttempts = 0;
            IsLocked = false;
            LockedAt = null;
            MarkAsDirty();
        }

        public void RecordFailedLogin()
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= 5)
            {
                IsLocked = true;
                LockedAt = DateTime.UtcNow;
            }
            MarkAsDirty();
        }

        public void UnlockAccount()
        {
            IsLocked = false;
            LockedAt = null;
            FailedLoginAttempts = 0;
            MarkAsDirty();
        }

        public void UpdatePassword(string passwordHash, string passwordSalt)
        {
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            ClearPasswordResetToken();
            MarkAsDirty();
        }

        public void SetPreferences(Dictionary<string, object> preferences)
        {
            Preferences = System.Text.Json.JsonSerializer.Serialize(preferences);
            MarkAsDirty();
        }

        public Dictionary<string, object> GetPreferences()
        {
            if (string.IsNullOrEmpty(Preferences))
                return new Dictionary<string, object>();

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(Preferences) 
                    ?? new Dictionary<string, object>();
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }
    }
}