namespace APSS.Application.DTOs;

public class FamilyIndividualDto
{
    public string FamilyName { get; set; } = null!;
    public string NameIndividual { get; set; } = null!;
    public bool IsParent { get; set; }
    public bool IsProvider { get; set; }
}