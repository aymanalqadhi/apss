namespace APSS.Web.Dto
{
    public class FamilyDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string LivingLocation { get; set; } = null!;
        public string User { get; set; } = null!;
    }
}