namespace APSS.Web.Dtos;

public abstract class BaseDto
{
    /// <summary>
    /// Gets or sets the id of the entity
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the entity
    /// </summary>
    public DateTime CreatedAt { get; set; }
}