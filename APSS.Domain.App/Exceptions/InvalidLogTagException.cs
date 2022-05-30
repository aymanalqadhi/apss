namespace APSS.Application.App.Exceptions;

public sealed class InvalidLogTagException : Exception
{
    #region Private fields

    private readonly string _tag;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="tag">The failing tag value</param>
    public InvalidLogTagException(string tag)
        : base($"invalid tag value `{tag}'")
    {
        _tag = tag;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the failing tag value
    /// </summary>
    public string Tag => _tag;

    #endregion
}
