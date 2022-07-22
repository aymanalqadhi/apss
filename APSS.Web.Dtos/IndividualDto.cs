namespace APSS.Web.Dto
{
    public class IndividualDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Job { get; set; } = null!;
        public string Sex { get; set; }= null!;
        public string SocialStatus { get; set; } = null!;
        public string PhonNumber { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string User { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
}
