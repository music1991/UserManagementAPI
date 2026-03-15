using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var currentUserId = int.Parse(User.FindFirst("Id")!.Value);
        var users = await _userService.GetAllAsync(currentUserId);

        var result = users.Select(u => new
        {
            u.Id,
            u.Email,
            Role = u.Role.ToString()
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound();

        var response = new ResponseProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            Addresses = user.Addresses.Select(a => new ResponseAddressDto
            {
                Id = a.Id,
                Street = a.Street,
                City = a.City,
                Country = a.Country
            }).ToList(),
            Studies = user.Studies.Select(s => new ResponseStudyDto
            {
                Id = s.Id,
                Title = s.Title,
                Institution = s.Institution,
                CompletionDate = s.CompletionDate
            }).ToList()
        };

        return Ok(response);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = int.Parse(User.FindFirst("Id")!.Value);
        var user = await _userService.GetProfileByIdAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            Role = user.Role.ToString(),
            user.Addresses,
            user.Studies
        });
    }

    [HttpPut("{id}/email")]
    public async Task<IActionResult> UpdateUserEmail(int id, [FromBody] UpdateProfileEmailRequest request)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")!.Value);
        Enum.TryParse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value, true, out var currentUserRole);

        var result = await _userService.UpdateEmailAsync(id, currentUserId, currentUserRole, request);

        if (!result.Success)
        {
            return result.Error switch
            {
                "FORBIDDEN" => Forbid(),
                "NOT_FOUND" => NotFound(),
                "EMAIL_EXISTS" => BadRequest("Email ya registrado"),
                _ => BadRequest()
            };
        }

        return NoContent();
    }

    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleRequest request)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")!.Value);

        if (id == currentUserId)
            return BadRequest("No puedes cambiar tu propio rol por seguridad.");

        var result = await _userService.UpdateUserRoleAsync(id, request.NewRole);

        if (!result.Success)
        {
            return result.Error switch
            {
                "NOT_FOUND" => NotFound(),
                "INVALID_ROLE" => BadRequest("El rol especificado no es válido."),
                _ => BadRequest()
            };
        }

        return NoContent();
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var result = await _userService.DeleteUserAsync(userId);

        if (!result.Success)
        {
            return result.Error switch
            {
                "NOT_FOUND" => NotFound(),
                "FORBIDDEN" => Forbid(),
                "SELF_DELETE" => BadRequest("No puedes eliminar tu propia cuenta."),
                _ => BadRequest()
            };
        }

        return NoContent();
    }
}