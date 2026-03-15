using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync(int currentUserId);
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetProfileByIdAsync(int userId);
    Task<(bool Success, string? Error)> UpdateUserRoleAsync(int userId, UserRole newRole);
    Task<(bool Success, string? Error)> UpdateEmailAsync(int targetUserId, int currentUserId, UserRole currentUserRole, UpdateProfileEmailRequest request);
    Task<(bool Success, string? Error)> DeleteUserAsync(int userId);
}