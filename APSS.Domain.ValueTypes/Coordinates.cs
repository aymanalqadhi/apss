using APSS.Domain.ValueTypes.Exceptions;

namespace APSS.Domain.ValueTypes;

/// <summary>
/// A value type to represent phyiscal coordinates
/// </summary>
public sealed record Coordinates
{
    private readonly double _latitude;
    private readonly double _longitude;

    /// <summary>
    /// Default construtor
    /// </summary>
    /// <param name="latitude">The coordinates latitude value</param>
    /// <param name="longitude">The coordinates longitude value</param>
    public Coordinates(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
            throw new InvalidCoordinatesException(latitude, longitude);

        _latitude = latitude;
        _longitude = longitude;
    }

    /// <summary>
    /// Gets the latitude value of the coordinates
    /// </summary>
    public double Latitude => _latitude;

    /// <summary>
    /// Gets the longitude value of the coordinates
    /// </summary>
    public double Longitude => _longitude;
}