using APSS.Domain.Entities;

namespace APSS.Domain.Services.Exceptions;

public sealed class UnsufficientPermissionException : Exception
{
    #region Fields

    private readonly User _user;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="user">The user who lacks permissions</param>
    /// <param name="message">An optional message to clarify the error</param>
    public UnsufficientPermissionException(User user, string? message)
        : base(message)
    {
        _user = user;
    }

    #endregion Public Constructors

    #region Properties

    /// <summary>
    /// Gets the user who lacks sufficient permissions
    /// </summary>
    public User User => _user;

    #endregion Properties
}