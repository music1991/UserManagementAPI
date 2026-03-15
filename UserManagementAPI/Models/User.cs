using System.ComponentModel.DataAnnotations;
using System.Net;

namespace UserManagementAPI.Models;
public class User
{
    [Key]
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.User;
    public List<Address> Addresses { get; set; } = new();
    public List<Study> Studies { get; set; } = new();
}
