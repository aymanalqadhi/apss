namespace APSS.Domain.Entities;

public class LandProductUnit : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of animal product unit
    /// </summary>
    public string Name { get; set; } = null!;
}