using APSS.Domain.ValueTypes.Exceptions;

namespace APSS.Domain.ValueTypes;

public struct DateTimeRange
{
    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="start">The starting date/time of the range</param>
    /// <param name="end">The ending date/time of the range</param>
    public DateTimeRange(DateTime start, DateTime end)
    {
        if (start > end)
            throw new InvalidDateTimeRangeException(start, end);

        Start = start;
        End = end;
    }

    #endregion
    
    #region Public properties

    /// <summary>
    /// Gets the start of the range
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Gets the end of the range
    /// </summary>
    public DateTime End { get; }

    #endregion
}