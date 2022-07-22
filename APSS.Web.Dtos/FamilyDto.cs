namespace APSS.Web.Dtos
{
    public class FamilyDto : BaseAuditbleDto
    {
        public string Name { get; set; } = null!;
        public string LivingLocation { get; set; } = null!;
        public UserDto? User { get; set; }
    }
}