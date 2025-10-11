using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a user session for tracking login sessions
    /// </summary>
    public class UserSession : BaseEntity
    {
        /// <summary>
        /// Reference to the user
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Session token (JWT or similar)
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string SessionToken { get; set; } = string.Empty;

        /// <summary>
        /// Refresh token for extending sessions
        /// </summary>
        [StringLength(1000)]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// When the session expires
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// When the refresh token expires
        /// </summary>
        public DateTime? RefreshTokenExpiresAt { get; set; }

        /// <summary>
        /// IP address of the session
        /// </summary>
        [StringLength(45)] // IPv6 max length
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent string
        /// </summary>
        [StringLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Device information
        /// </summary>
        [StringLength(200)]
        public string? DeviceInfo { get; set; }

        /// <summary>
        /// Whether the session is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// When the session was last accessed
        /// </summary>
        public DateTime? LastAccessedAt { get; set; }

        /// <summary>
        /// When the session was revoked (if applicable)
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// Reason for session revocation
        /// </summary>
        [StringLength(200)]
        public string? RevocationReason { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;

        // Computed Properties
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;

        [NotMapped]
        public bool IsRefreshTokenExpired => 
            RefreshTokenExpiresAt.HasValue && DateTime.UtcNow > RefreshTokenExpiresAt.Value;

        [NotMapped]
        public bool IsValid => IsActive && !IsExpired && RevokedAt == null;

        [NotMapped]
        public bool CanRefresh => 
            IsActive && 
            !string.IsNullOrEmpty(RefreshToken) && 
            !IsRefreshTokenExpired && 
            RevokedAt == null;

        // Methods
        public void UpdateLastAccessed()
        {
            LastAccessedAt = DateTime.UtcNow;
            MarkAsDirty();
        }

        public void Revoke(string reason = "Manual revocation")
        {
            IsActive = false;
            RevokedAt = DateTime.UtcNow;
            RevocationReason = reason;
            MarkAsDirty();
        }

        public void ExtendSession(TimeSpan extension)
        {
            ExpiresAt = DateTime.UtcNow.Add(extension);
            UpdateLastAccessed();
        }

        public void UpdateRefreshToken(string refreshToken, DateTime expiresAt)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiresAt = expiresAt;
            MarkAsDirty();
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiresAt = null;
            MarkAsDirty();
        }
    }
}