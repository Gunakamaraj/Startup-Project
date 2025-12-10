using PRM.Dtos;
using PRM.Models;

namespace PRM.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterDtos registerDto);
        Task<(bool Success, string Message, User? User)> LoginAsync(LoginDtos loginDto);
        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<(bool Success, string Message)> UpdateUserAsync(int id, User user);
        Task<(bool Success, string Message)> DeleteUserAsync(int id);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
