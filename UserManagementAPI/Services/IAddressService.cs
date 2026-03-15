using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public interface IAddressService
{
    Task<IEnumerable<Address>> GetMyAddressesAsync(int userId);
    Task<(bool Success, string? Error, Address? Address)> AddAddressAsync(int currentUserId, UserRole currentUserRole, CreateAddressDto dto);
    Task<(bool Success, string? Error)> UpdateAddressAsync(int id, int currentUserId, UserRole currentUserRole, UpdateAddressDto dto);
    Task<(bool Success, string? Error)> DeleteAddressAsync(int id, int currentUserId, UserRole currentUserRole);
}
