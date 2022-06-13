namespace APSS.Domain.Services.Exceptions;

public sealed class InvalidDateTimeException : Exception
{
    #region Fields

    private readonly DateTime _dateTime;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="dateTime">The faulty date/time</param>
    /// <param name="message">An optional message to clarify the error</param>
    public InvalidDateTimeException(DateTime dateTime, string? message = null)
        : base(message)
    {
        _dateTime = dateTime;
    }

    #endregion Public Constructors

    #region Properties

    /// <summary>
    /// Gets the faulty date/time
    /// </summary>
    public DateTime DateTime => _dateTime;

    #endregion Properties
}