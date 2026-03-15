namespace UserManagementAPI.DTOs;

public class CreateStudyDto
{
    public int? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
}
