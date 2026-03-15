using UserManagementAPI.Models;

namespace UserManagementAPI.DTOs;

public class UpdateRoleRequest
{
    public UserRole NewRole { get; set; }
}