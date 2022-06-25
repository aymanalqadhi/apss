using System.Security.AccessControl;

namespace APSS.Domain.Services.Exceptions;

/// <summary>
/// An exception to be thrown when a user account lacks sufficient permissions to perform an operation
/// </summary>
public sealed class InsufficientPermissionsException : Exception
{
    #region Fields

    private readonly long _accountId;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="accountId">The id of the account who lacks permissions</param>
    /// <param name="message">An optional message to clarify the error</param>
    public InsufficientPermissionsException(long accountId, string? message)
        : base(message)
    {
        _accountId = accountId;
    }

    #endregion Public Constructors

    #region Properties

    /// <summary>
    /// Gets the id of the user who lacks sufficient permissions
    /// </summary>
    public long AccountId => _accountId;

    #endregion Properties
}