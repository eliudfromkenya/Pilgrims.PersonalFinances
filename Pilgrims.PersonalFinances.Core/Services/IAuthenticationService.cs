using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Services
{
    /// <summary>
    /// Interface for authentication services
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticate a user with email and password
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns>Authentication result with user and session information</returns>
        Task<AuthenticationResult> AuthenticateAsync(string email, string password);

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <returns>Registration result</returns>
        Task<RegistrationResult> RegisterAsync(string email, string password, string firstName, string lastName);

        /// <summary>
        /// Refresh an authentication token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>New authentication result</returns>
        Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Logout a user by revoking their session
        /// </summary>
        /// <param name="sessionToken">Session token to revoke</param>
        /// <returns>True if successful</returns>
        Task<bool> LogoutAsync(string sessionToken);

        /// <summary>
        /// Verify email with verification token
        /// </summary>
        /// <param name="token">Email verification token</param>
        /// <returns>True if successful</returns>
        Task<bool> VerifyEmailAsync(string token);

        /// <summary>
        /// Request password reset
        /// </summary>
        /// <param name="email">User's email</param>
        /// <returns>True if successful</returns>
        Task<bool> RequestPasswordResetAsync(string email);

        /// <summary>
        /// Reset password with reset token
        /// </summary>
        /// <param name="token">Password reset token</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True if successful</returns>
        Task<bool> ResetPasswordAsync(string token, string newPassword);

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="currentPassword">Current password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True if successful</returns>
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        /// <summary>
        /// Get user by session token
        /// </summary>
        /// <param name="sessionToken">Session token</param>
        /// <returns>User if valid session</returns>
        Task<User?> GetUserBySessionTokenAsync(string sessionToken);

        /// <summary>
        /// Validate session token
        /// </summary>
        /// <param name="sessionToken">Session token</param>
        /// <returns>True if valid</returns>
        Task<bool> ValidateSessionTokenAsync(string sessionToken);

        /// <summary>
        /// Get all active sessions for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of active sessions</returns>
        Task<IEnumerable<UserSession>> GetActiveSessionsAsync(string userId);

        /// <summary>
        /// Revoke all sessions for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="reason">Revocation reason</param>
        /// <returns>Number of sessions revoked</returns>
        Task<int> RevokeAllSessionsAsync(string userId, string reason = "All sessions revoked");

        /// <summary>
        /// Clean up expired sessions
        /// </summary>
        /// <returns>Number of sessions cleaned up</returns>
        Task<int> CleanupExpiredSessionsAsync();
    }

    /// <summary>
    /// Result of authentication attempt
    /// </summary>
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public User? User { get; set; }
        public string? SessionToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }

        public static AuthenticationResult Success(User user, string sessionToken, string refreshToken, 
            DateTime expiresAt, DateTime refreshTokenExpiresAt)
        {
            return new AuthenticationResult
            {
                IsSuccess = true,
                User = user,
                SessionToken = sessionToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }

        public static AuthenticationResult Failure(string errorMessage)
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Result of registration attempt
    /// </summary>
    public class RegistrationResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public User? User { get; set; }
        public string? EmailVerificationToken { get; set; }

        public static RegistrationResult Success(User user, string emailVerificationToken)
        {
            return new RegistrationResult
            {
                IsSuccess = true,
                User = user,
                EmailVerificationToken = emailVerificationToken
            };
        }

        public static RegistrationResult Failure(string errorMessage)
        {
            return new RegistrationResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}