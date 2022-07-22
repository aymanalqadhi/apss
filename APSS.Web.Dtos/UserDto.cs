using APSS.Domain.Entities;

namespace APSS.Web.Dto;

public sealed class UserDto : BaseAuditbleDto
{
    /// <summary>
    /// Gets or stes the name of the user
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or stes the access level of the user
    /// </summary>
    public AccessLevel AccessLevel { get; set; }

    /// <summary>
    /// Gets or sets the user status
    /// </summary>
    public UserStatus UserStatus { get; set; }

    /// <summary>
    /// Gets or sets the supervisor
    /// </summary>
    public UserDto? SupervisedBy { get; set; }
}