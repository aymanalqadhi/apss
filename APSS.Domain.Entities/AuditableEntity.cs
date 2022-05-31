namespace APSS.Domain.Entities;

public abstract class AuditableEntity
{
    /// <summary>
    /// Gets or sets the id of the entity
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the entity
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last modification date of the entity
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}