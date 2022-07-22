namespace APSS.Web.Dto
{
    public class FamilyIndividualDto
    {
        public long Id { get; set; }
        public string Individual { get; set; } = null!;
        public bool IsProvider { get; set; } = false;
        public bool IsParent { get; set; }
    }
}