using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _context;

    public AddressService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Address>> GetMyAddressesAsync(int userId)
    {
        return await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task<(bool Success, string? Error, Address? Address)> AddAddressAsync(int currentUserId, UserRole currentUserRole, CreateAddressDto dto)
    {
        var targetUserId = dto.UserId ?? currentUserId;
        var isAdmin = currentUserRole == UserRole.Admin;
        var isOwner = targetUserId == currentUserId;

        if (!isOwner && !isAdmin)
            return (false, "FORBIDDEN", null);

        var userExists = await _context.Users.AnyAsync(u => u.Id == targetUserId);
        if (!userExists)
            return (false, "USER_NOT_FOUND", null);

        var address = new Address
        {
            Street = dto.Street,
            City = dto.City,
            Country = dto.Country,
            UserId = targetUserId
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return (true, null, address);
    }

    public async Task<(bool Success, string? Error)> UpdateAddressAsync(int id, int currentUserId, UserRole currentUserRole,UpdateAddressDto dto)
    {
        var address = await _context.Addresses.FindAsync(id);

        if (address == null)
            return (false, "NOT_FOUND");

        var isOwner = address.UserId == currentUserId;
        var isAdmin = currentUserRole == UserRole.Admin;

        if (!isOwner && !isAdmin)
            return (false, "FORBIDDEN");

        if (dto.Street is not null)
            address.Street = dto.Street;

        if (dto.City is not null)
            address.City = dto.City;

        if (dto.Country is not null)
            address.Country = dto.Country;

        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAddressAsync(int id, int currentUserId, UserRole currentUserRole)
    {
        var address = await _context.Addresses.FindAsync(id);

        if (address == null)
            return (false, "NOT_FOUND");

        var isOwner = address.UserId == currentUserId;
        var isAdmin = currentUserRole == UserRole.Admin;

        if (!isOwner && !isAdmin)
            return (false, "FORBIDDEN");

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();

        return (true, null);
    }
}
