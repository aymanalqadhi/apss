namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a land
/// </summary>
public sealed class Land : Confirmable
{
    /// <summary>
    /// Gets or sets the name of the land
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the area of the land in meters
    /// </summary>
    public long Area { get; set; }

    /// <summary>
    /// Gets or sets the longitude of the land
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Gets or sets the Latitude of the land
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the address of the land
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Gets or sets whether the land can be used or not
    /// </summary>
    public bool IsUsable { get; set; }

    /// <summary>
    /// Gets or sets whether the land is used or not
    /// </summary>
    public bool IsUsed { get; set; } = false;

    /// <summary>
    /// Gets or sets the owner of the land
    /// </summary>
    public User OwnedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of products produced by this land
    /// </summary>
    public ICollection<LandProduct> Products { get; set; } = new List<LandProduct>();
}