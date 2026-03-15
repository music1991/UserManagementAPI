using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTOs;

public class UpdateProfileEmailRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}