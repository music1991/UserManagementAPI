namespace UserManagementAPI.Models;

public class SessionLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
}
