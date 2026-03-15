using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto request)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (response == null)
            return Unauthorized(new { message = "Sesión expirada, por favor inicie sesión nuevamente" });

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RequestRegisterDto request)
    {
        var newUser = await _authService.RegisterAsync(request);
        if (newUser == null) return BadRequest("El email ya existe.");
        return Ok(new { message = "Registrado con éxito" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RequestLoginDto request)
    {
        var response = await _authService.LoginAsync(request);

        if (response == null)
            return Unauthorized(new { message = "Correo o Contraseña incorrectos" });

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value!);

        await _authService.LogoutAsync(userId);

        return Ok(new { message = "Sesión cerrada correctamente" });
    }

    [HttpPost("update-password")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        var isAdmin = User.IsInRole("Admin");

        if (currentUserEmail != request.Email && !isAdmin)
        {
            return Forbid();
        }

        var result = await _authService.UpdatePasswordAsync(request, isAdmin);

        if (!result) return BadRequest();

        return Ok(new { message = "Contraseña actualizada" });
    }
}