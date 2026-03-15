using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync(int currentUserId)
    {
        return await _context.Users
            .Where(u => u.Id != currentUserId)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.Studies)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetProfileByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.Studies)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<(bool Success, string? Error)> UpdateUserRoleAsync(int targetUserId, UserRole newRole)
    {
        var user = await _context.Users.FindAsync(targetUserId);

        if (user == null)
            return (false, "NOT_FOUND");

        user.Role = newRole;

        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateEmailAsync(int targetUserId, int currentUserId, UserRole role, UpdateProfileEmailRequest request)
    {
        if (role != UserRole.Admin && currentUserId != targetUserId)
            return (false, "FORBIDDEN");

        var user = await _context.Users.FindAsync(targetUserId);
        if (user == null)
            return (false, "NOT_FOUND");

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email && u.Id != targetUserId);

        if (emailExists)
            return (false, "EMAIL_EXISTS");

        user.Email = request.Email;
        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return (false, "NOT_FOUND");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return (true, null);
    }
}
