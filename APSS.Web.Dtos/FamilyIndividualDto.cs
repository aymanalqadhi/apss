namespace APSS.Web.Dtos
{
    public class FamilyIndividualDto : BaseAuditbleDto
    {
        public FamilyDto Family { get; set; } = null!;
        public IndividualDto Individual { get; set; } = null!;
        public bool IsProvider { get; set; } = false;
        public bool IsParent { get; set; }
    }
}