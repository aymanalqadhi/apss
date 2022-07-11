namespace APSS.Domain.ValueTypes.Exceptions;

/// <summary>
/// An exception to be thrown when invalid coordinates are used
/// </summary>
public sealed class InvalidCoordinatesException : Exception
{
    #region Private Fields

    private readonly double _latitude;
    private readonly double _longitude;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    /// Default construtor
    /// </summary>
    /// <param name="latitude">The coordinates latitude value</param>
    /// <param name="longitude">The coordinates longitude value</param>
    public InvalidCoordinatesException(double latitude, double longitude)
        : base($"invalid coordinates ({latitude}:{longitude}}")
    {
        _latitude = latitude;
        _longitude = longitude;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// Gets the latitude value of the coordinates
    /// </summary>
    public double Latitude => _latitude;

    /// <summary>
    /// Gets the longitude value of the coordinates
    /// </summary>
    public double Longitude => _longitude;

    #endregion Public Properties
}