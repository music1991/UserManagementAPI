using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public interface IAuthService
{
    Task<ResponseAuthDto?> LoginAsync(RequestLoginDto request);
    Task<User?> RegisterAsync(RequestRegisterDto request);
    Task LogoutAsync(int userId);
    Task<bool> UpdatePasswordAsync(UpdatePasswordRequest request, bool isAdmin);
    Task<ResponseAuthDto?> RefreshTokenAsync(string refreshToken);
}
