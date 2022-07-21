namespace APSS.Application.DTOs;

public class IndividualDto
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Job { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Sex { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string NationalId { get; set; } = null!;
    public string SocialStatus { get; set; } = null!;
    public string AddedBy { get; set; } = null!;
}