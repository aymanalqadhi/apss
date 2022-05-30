namespace APSS.Domain.ValueTypes.Exceptions;

public class InvalidDateTimeRangeException : Exception
{
    #region Private fields

    private readonly DateTime _start;
    private readonly DateTime _end;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="start">The starting date/time of the range</param>
    /// <param name="end">The ending date/time of the range</param>
    public InvalidDateTimeRangeException(DateTime start, DateTime end)
        : base($"invalid date/time range: ${start}-${end}")
    {
        _start = start;
        _end = end;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the starting date/time of the errnous range
    /// </summary>
    public DateTime Start => _start;

    /// <summary>
    /// Gets the ending date/time of the erronous range
    /// </summary>
    public DateTime End => _end;

    #endregion
}
