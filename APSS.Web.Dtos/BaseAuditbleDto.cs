namespace APSS.Web.Dtos;

public abstract class BaseAuditbleDto : BaseDto
{
    /// <summary>
    /// Gets or sets the last modification date of the entity
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}