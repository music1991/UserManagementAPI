using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class StudyService : IStudyService
{
    private readonly ApplicationDbContext _context;

    public StudyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ResponseStudyDto>> GetStudiesByUserIdAsync(int userId)
    {
        return await _context.Studies
            .Where(s => s.UserId == userId)
            .Select(s => new ResponseStudyDto
            {
                Id = s.Id,
                Title = s.Title,
                Institution = s.Institution,
                CompletionDate = s.CompletionDate
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error, int? StudyId)> AddStudyAsync(
        int currentUserId,
        UserRole currentUserRole,
        CreateStudyDto dto)
    {
        var targetUserId = dto.UserId ?? currentUserId;
        var isAdmin = currentUserRole == UserRole.Admin;
        var isOwner = targetUserId == currentUserId;

        if (!isOwner && !isAdmin)
            return (false, "FORBIDDEN", null);

        var userExists = await _context.Users.AnyAsync(u => u.Id == targetUserId);
        if (!userExists)
            return (false, "USER_NOT_FOUND", null);

        var study = new Study
        {
            UserId = targetUserId,
            Title = dto.Title,
            Institution = dto.Institution,
            CompletionDate = dto.CompletionDate
        };

        _context.Studies.Add(study);
        await _context.SaveChangesAsync();

        return (true, null, study.Id);
    }

    public async Task<(bool Success, string? Error)> UpdateStudyAsync(
        int studyId,
        int currentUserId,
        UserRole currentUserRole,
        UpdateStudyDto dto)
    {
        var study = await _context.Studies.FirstOrDefaultAsync(s => s.Id == studyId);

        if (study == null)
            return (false, "NOT_FOUND");

        var isOwner = study.UserId == currentUserId;
        var isAdmin = currentUserRole == UserRole.Admin;

        if (!isOwner && !isAdmin)
            return (false, "FORBIDDEN");

        if (dto.Title is not null)
            study.Title = dto.Title;

        if (dto.Institution is not null)
            study.Institution = dto.Institution;

        if (dto.CompletionDate.HasValue)
            study.CompletionDate = dto.CompletionDate.Value;

        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteStudyAsync(
        int studyId,
        int currentUserId,
        UserRole currentUserRole)
    {
        var study = await _context.Studies.FindAsync(studyId);

        if (study == null)
            return (false, "NOT_FOUND");

        var isOwner = study.UserId == currentUserId;
        var isAdmin = currentUserRole == UserRole.Admin;

        if (!isOwner && !isAdmin)
            return (false, "FORBIDDEN");

        _context.Studies.Remove(study);
        await _context.SaveChangesAsync();

        return (true, null);
    }
}