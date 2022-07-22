namespace APSS.Web.Dto
{
    public class SkillDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Field { get; set; } = null!;
        public string NameIndividual { get; set; } = null!;
    }
}