using APSS.Web.Dtos;

namespace APSS.Web.Dto
{
    public class SkillDto : BaseAuditbleDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Field { get; set; } = null!;
        public IndividualDto Individual { get; set; } = null!;
    }
}