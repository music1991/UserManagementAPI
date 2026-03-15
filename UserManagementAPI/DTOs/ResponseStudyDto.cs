namespace UserManagementAPI.DTOs;

public class ResponseStudyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
}