namespace UserManagementAPI.DTOs;
public class UpdateStudyDto
{
    public string? Title { get; set; }
    public string? Institution { get; set; }
    public DateTime? CompletionDate { get; set; }
}