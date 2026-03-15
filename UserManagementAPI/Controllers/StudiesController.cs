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
public class StudiesController : ControllerBase
{
    private readonly IStudyService _studyService;

    public StudiesController(IStudyService studyService)
    {
        _studyService = studyService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyStudies()
    {
        var userId = int.Parse(User.FindFirst("Id")!.Value);

        var studies = await _studyService.GetStudiesByUserIdAsync(userId);

        return Ok(studies);
    }

    [HttpPost]
    public async Task<IActionResult> AddStudy([FromBody] CreateStudyDto dto)
    {
        var currentUserIdString = User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(currentUserIdString))
            return Unauthorized();

        var currentUserId = int.Parse(currentUserIdString);
        Enum.TryParse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value, true, out var currentUserRole);

        var result = await _studyService.AddStudyAsync(currentUserId, currentUserRole, dto);

        if (!result.Success)
        {
            return result.Error switch
            {
                "FORBIDDEN" => Forbid(),
                "USER_NOT_FOUND" => NotFound("Usuario no encontrado"),
                _ => BadRequest()
            };
        }

        return Ok(new
        {
            message = "Estudio agregado con éxito",
            studyId = result.StudyId
        });
    }

    [HttpPatch("{studyId}")]
    public async Task<IActionResult> UpdateStudy(int studyId, [FromBody] UpdateStudyDto dto)
    { 
        var currentUserId = int.Parse(User.FindFirst("Id")!.Value);
        Enum.TryParse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value, true, out var currentUserRole);

        var result = await _studyService.UpdateStudyAsync(studyId, currentUserId, currentUserRole, dto);

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

    [HttpDelete("{studyId}")]
    public async Task<IActionResult> Delete(int studyId)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")!.Value);
        Enum.TryParse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value, true, out var currentUserRole);

        var result = await _studyService.DeleteStudyAsync(studyId, currentUserId, currentUserRole);

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

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetStudyByUserId(int userId)
    {
        var address = await _studyService.GetStudiesByUserIdAsync(userId);

        if (address == null)
            return NotFound();

        return Ok(address);
    }
}