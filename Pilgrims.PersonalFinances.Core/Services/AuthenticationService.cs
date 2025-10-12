using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace Pilgrims.PersonalFinances.Core.Services
{
    /// <summary>
    /// Implementation of authentication services
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly PersonalFinanceContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            PersonalFinanceContext context,
            IConfiguration configuration,
            ILogger<AuthenticationService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("Authentication failed: User not found for email {Email}", email);
                    return AuthenticationResult.Failure("Invalid email or password");
                }

                if (!user.CanLogin)
                {
                    var reason = !user.IsActive ? "Account is inactive" :
                                user.IsLocked ? "Account is locked" :
                                !user.IsEmailVerified ? "Email not verified" : "Account access denied";
                    
                    _logger.LogWarning("Authentication failed: {Reason} for user {UserId}", reason, user.Id);
                    return AuthenticationResult.Failure(reason);
                }

                if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                {
                    user.RecordFailedLogin();
                    await _context.SaveChangesAsync();
                    
                    _logger.LogWarning("Authentication failed: Invalid password for user {UserId}", user.Id);
                    return AuthenticationResult.Failure("Invalid email or password");
                }

                // Create session
                var sessionToken = GenerateSessionToken();
            var refreshToken = GenerateRefreshToken();
                var expiresAt = DateTime.UtcNow.AddHours(24);
                var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(30);

                var session = new UserSession
                {
                    UserId = user.Id,
                    SessionToken = sessionToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    RefreshTokenExpiresAt = refreshTokenExpiresAt,
                    IpAddress = "127.0.0.1", // You might want to get this from HttpContext
                    UserAgent = "Personal Finance App",
                    IsActive = true,
                    LastAccessedAt = DateTime.UtcNow
                };

                _context.UserSessions.Add(session);
                user.RecordSuccessfulLogin();
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} authenticated successfully", user.Id);

                return AuthenticationResult.Success(user, sessionToken, refreshToken, expiresAt, refreshTokenExpiresAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for email {Email}", email);
                return AuthenticationResult.Failure("An error occurred during authentication");
            }
        }

        public async Task<RegistrationResult> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: User already exists for email {Email}", email);
                    return RegistrationResult.Failure("A user with this email already exists");
                }

                // Validate password strength
                if (!IsPasswordStrong(password))
                {
                    return RegistrationResult.Failure("Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character");
                }

                // Create password hash
                var (passwordHash, passwordSalt) = HashPassword(password);

                // Create user
                var user = new User
                {
                    Email = email.ToLower(),
                    FirstName = firstName,
                    LastName = lastName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    IsActive = true,
                    IsEmailVerified = false,
                    CreatedAt = DateTime.UtcNow
                    // UpdatedAt will be set automatically by BaseEntity
                };

                user.GenerateEmailVerificationToken();

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User registered successfully with ID {UserId}", user.Id);

                return RegistrationResult.Success(user, user.EmailVerificationToken!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email {Email}", email);
                return RegistrationResult.Failure("An error occurred during registration");
            }
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var session = await _context.UserSessions
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.IsActive);

                if (session == null || !session.CanRefresh)
                {
                    _logger.LogWarning("Refresh token failed: Invalid or expired token");
                    return AuthenticationResult.Failure("Invalid or expired refresh token");
                }

                // Generate new tokens
                var newSessionToken = GenerateSessionToken();
                var newRefreshToken = GenerateRefreshToken();
                var expiresAt = DateTime.UtcNow.AddHours(24);
                var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(30);

                // Update session
                session.SessionToken = newSessionToken;
                session.UpdateRefreshToken(newRefreshToken, refreshTokenExpiresAt);
                session.ExtendSession(TimeSpan.FromHours(24));

                await _context.SaveChangesAsync();

                _logger.LogInformation("Token refreshed successfully for user {UserId}", session.UserId);

                return AuthenticationResult.Success(session.User, newSessionToken, newRefreshToken, expiresAt, refreshTokenExpiresAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return AuthenticationResult.Failure("An error occurred during token refresh");
            }
        }

        public async Task<bool> LogoutAsync(string sessionToken)
        {
            try
            {
                var session = await _context.UserSessions
                    .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.IsActive);

                if (session != null)
                {
                    session.Revoke("User logout");
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("User {UserId} logged out successfully", session.UserId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailVerificationToken == token && u.IsEmailVerificationTokenValid);

                if (user != null)
                {
                    user.MarkEmailAsVerified();
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Email verified successfully for user {UserId}", user.Id);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email verification");
                return false;
            }
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user != null)
                {
                    user.GeneratePasswordResetToken();
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Password reset requested for user {UserId}", user.Id);
                    // TODO: Send password reset email
                }

                // Always return true to prevent email enumeration
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset request");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.PasswordResetToken == token && u.IsPasswordResetTokenValid);

                if (user == null)
                {
                    return false;
                }

                if (!IsPasswordStrong(newPassword))
                {
                    return false;
                }

                var (passwordHash, passwordSalt) = HashPassword(newPassword);
                user.UpdatePassword(passwordHash, passwordSalt);

                // Revoke all existing sessions
                await RevokeAllSessionsAsync(user.Id, "Password reset");

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                if (!VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    return false;
                }

                if (!IsPasswordStrong(newPassword))
                {
                    return false;
                }

                var (passwordHash, passwordSalt) = HashPassword(newPassword);
                user.UpdatePassword(passwordHash, passwordSalt);

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change for user {UserId}", userId);
                return false;
            }
        }

        public async Task<User?> GetUserBySessionTokenAsync(string sessionToken)
        {
            try
            {
                var session = await _context.UserSessions
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.IsValid);

                if (session != null)
                {
                    session.UpdateLastAccessed();
                    await _context.SaveChangesAsync();
                    return session.User;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by session token");
                return null;
            }
        }

        public async Task<bool> ValidateSessionTokenAsync(string sessionToken)
        {
            try
            {
                var session = await _context.UserSessions
                    .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.IsValid);

                if (session != null)
                {
                    session.UpdateLastAccessed();
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating session token");
                return false;
            }
        }

        public async Task<IEnumerable<UserSession>> GetActiveSessionsAsync(string userId)
        {
            try
            {
                return await _context.UserSessions
                    .Where(s => s.UserId == userId && s.IsValid)
                    .OrderByDescending(s => s.LastAccessedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active sessions for user {UserId}", userId);
                return Enumerable.Empty<UserSession>();
            }
        }

        public async Task<int> RevokeAllSessionsAsync(string userId, string reason = "All sessions revoked")
        {
            try
            {
                var sessions = await _context.UserSessions
                    .Where(s => s.UserId == userId && s.IsActive)
                    .ToListAsync();

                foreach (var session in sessions)
                {
                    session.Revoke(reason);
                }

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Revoked {Count} sessions for user {UserId}", sessions.Count, userId);
                return sessions.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking sessions for user {UserId}", userId);
                return 0;
            }
        }

        public async Task<int> CleanupExpiredSessionsAsync()
        {
            try
            {
                var expiredSessions = await _context.UserSessions
                    .Where(s => s.IsExpired || (s.RefreshTokenExpiresAt.HasValue && s.RefreshTokenExpiresAt.Value < DateTime.UtcNow))
                    .ToListAsync();

                foreach (var session in expiredSessions)
                {
                    session.Revoke("Session expired");
                }

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Cleaned up {Count} expired sessions", expiredSessions.Count);
                return expiredSessions.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired sessions");
                return 0;
            }
        }

        #region Private Methods

        private (string hash, string salt) HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[32];
            rng.GetBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
            var hash = Convert.ToBase64String(pbkdf2.GetBytes(32));

            return (hash, salt);
        }

        private bool VerifyPassword(string password, string hash, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
            var computedHash = Convert.ToBase64String(pbkdf2.GetBytes(32));
            return computedHash == hash;
        }

        private bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private string GenerateSessionToken()
        {
            // Generate a secure random token for session management
            using var rng = RandomNumberGenerator.Create();
            var tokenBytes = new byte[32];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }

        private string GenerateRefreshToken()
        {
            // Generate a secure random refresh token
            using var rng = RandomNumberGenerator.Create();
            var tokenBytes = new byte[32];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }

        #endregion
    }
}
