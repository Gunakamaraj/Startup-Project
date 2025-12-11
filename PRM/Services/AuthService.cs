using PRM.Data.Repositories;
using PRM.Dtos;
using PRM.Models;
using System.Security.Cryptography;
using System.Text;

namespace PRM.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterDtos registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.email);
                if (existingUser != null)
                {
                    _logger.LogWarning($"Registration failed: Email {registerDto.email} already exists");
                    return (false, "Email already registered", null);
                }

                // Create new user
                var user = new User
                {
                    FirstName = registerDto.firstName,
                    LastName = registerDto.lastName,
                    Email = registerDto.email,
                    Password = HashPassword(registerDto.password),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createdUser = await _userRepository.CreateUserAsync(user);
                _logger.LogInformation($"User registered successfully: {createdUser.Email}");

                return (true, "Registration successful", createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Registration error: {ex.Message}");
                return (false, $"Registration failed: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, User? User)> LoginAsync(LoginDtos loginDto)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetUserByEmailAsync(loginDto.email);
                if (user == null)
                {
                    _logger.LogWarning($"Login failed: User not found with email {loginDto.email}");
                    return (false, "Invalid email or password", null);
                }

                // Verify password
                if (!VerifyPassword(loginDto.password, user.Password))
                {
                    _logger.LogWarning($"Login failed: Invalid password for {loginDto.email}");
                    return (false, "Invalid email or password", null);
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning($"Login failed: User account is inactive {loginDto.email}");
                    return (false, "User account is inactive", null);
                }

                _logger.LogInformation($"User logged in successfully: {user.Email}");
                return (true, "Login successful", user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                return (false, $"Login failed: {ex.Message}", null);
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(int id, User user)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    return (false, "User not found");
                }

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.IsActive = user.IsActive;

                await _userRepository.UpdateUserAsync(existingUser);
                _logger.LogInformation($"User updated: {existingUser.Email}");

                return (true, "User updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update user error: {ex.Message}");
                return (false, $"Update failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            try
            {
                var deleted = await _userRepository.DeleteUserAsync(id);
                if (!deleted)
                {
                    return (false, "User not found");
                }

                _logger.LogInformation($"User deleted: {id}");
                return (true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete user error: {ex.Message}");
                return (false, $"Delete failed: {ex.Message}");
            }
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash);
        }
    }
}
