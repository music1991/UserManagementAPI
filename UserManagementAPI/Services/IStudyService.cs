using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public interface IStudyService
{
    Task<IEnumerable<ResponseStudyDto>> GetStudiesByUserIdAsync(int userId);
    Task<(bool Success, string? Error, int? StudyId)> AddStudyAsync(int currentUserId, UserRole currentUserRole, CreateStudyDto dto);
    Task<(bool Success, string? Error)> UpdateStudyAsync(int id, int currentUserId, UserRole currentUserRole, UpdateStudyDto dto);
    Task<(bool Success, string? Error)> DeleteStudyAsync(int studyId, int currentUserId, UserRole currentUserRole);
}