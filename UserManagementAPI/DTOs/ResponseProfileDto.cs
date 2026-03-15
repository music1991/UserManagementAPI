namespace UserManagementAPI.DTOs;

public class ResponseProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";

    public List<ResponseAddressDto> Addresses { get; set; } = new();
    public List<ResponseStudyDto> Studies { get; set; } = new();
}

