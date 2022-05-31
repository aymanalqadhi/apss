namespace APSS.Domain.Entities;

public abstract class Confirmable : AuditableEntity
{
    /// <summary>
    /// Gets or sets wether it's confirmed or not
    /// </summary>
    public bool IsConfirmed { get; set; } = false;
}