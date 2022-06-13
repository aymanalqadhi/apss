using APSS.Domain.Entities;

namespace APSS.Domain.Services.Exceptions;

public sealed class InsufficientPermissionsException : Exception
{
    #region Fields

    private readonly long _userId;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="userId">The id of the user who lacks permissions</param>
    /// <param name="message">An optional message to clarify the error</param>
    public InsufficientPermissionsException(long userId, string? message)
        : base(message)
    {
        _userId = userId;
    }

    #endregion Public Constructors

    #region Properties

    /// <summary>
    /// Gets the id of the user who lacks sufficient permissions
    /// </summary>
    public long UserId => _userId;

    #endregion Properties
}