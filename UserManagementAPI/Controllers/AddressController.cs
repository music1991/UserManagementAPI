using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyAddresses()
    {
        var currentUserId = int.Parse(User.FindFirst("Id")!.Value);
        var addresses = await _addressService.GetMyAddressesAsync(currentUserId);

        return Ok(addresses);
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress([FromBody] CreateAddressDto dto)
    {
        var currentUserIdString = User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(currentUserIdString))
            return Unauthorized();

        var currentUserId = int.Parse(currentUserIdString);
        Enum.TryParse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value, true, out var currentUserRole);

        var result = await _addressService.AddAddressAsync(currentUserId, currentUserRole, dto);

        if (!result.Success)
        {
            return result.Error switch
            {
                "FORBIDDEN" => Forbid(),
                "USER_NOT_FOUND" => NotFound("Usuario no encontrado"),
                _ => BadRequest(new { message = result.Error })
            };
        }

        return Ok(new
        {
            message = "Dirección agregada con éxito",
            address = result.Address
        });
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAddressesByUserId(int userId) // Cambiado a plural
    {
        var addresses = await _addressService.GetMyAddressesAsync(userId);
        return Ok(addresses);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressDto dto)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")!.Value);
        Enum.TryParse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value, true, out var currentUserRole);

        var result = await _addressService.UpdateAddressAsync(id, currentUserId, currentUserRole, dto);

        if (!result.Success)
        {
            return result.Error switch
            {
                "NOT_FOUND" => NotFound(),
                "FORBIDDEN" => Forbid(),
                _ => BadRequest()
            };
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")!.Value);
        Enum.TryParse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value, true, out var currentUserRole);

        var result = await _addressService.DeleteAddressAsync(id, currentUserId, currentUserRole);

        if (!result.Success)
        {
            return result.Error switch
            {
                "NOT_FOUND" => NotFound(),
                "FORBIDDEN" => Forbid(),
                _ => BadRequest()
            };
        }

        return NoContent();
    }
}
