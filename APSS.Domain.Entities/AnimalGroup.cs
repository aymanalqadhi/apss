namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent an animal group
/// </summary>
public sealed class AnimalGroup : Ownable
{
    /// <summary>
    /// Gets or sets the type of the animal group
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity of the animal group
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the sex of the animal group
    /// </summary>
    public AnimalSex Sex { get; set; }

    /// <summary>
    /// Gets or sets the products of the animal
    /// </summary>
    public ICollection<AnimalProduct> Products { get; set; } = new List<AnimalProduct>();
}

/// <summary>
/// An enum to represent the sex of the animal
/// </summary>
public enum AnimalSex
{
    Male,
    Female,
}