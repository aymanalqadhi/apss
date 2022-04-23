namespace APSS.Domain.Entities;

public sealed class User : AuditableEntity
{
    /// <summary>
    /// Gets or sets the number of the users
    /// </summary>
    public string UserNumber { get; set; } = null!;
}
