namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent the products of the land
/// </summary>
public sealed class LandProduct : Confirmable
{
    /// <summary>
    /// Gets or sets the storing method of the land's product
    /// </summary>
    public string StoringMethod { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unit of the land's product
    /// </summary>
    public string Unit { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity of the land's product
    /// </summary>
    public double Quantity { get; set; }

    /// <summary>
    /// Gets or sets the crop name of the land's product
    /// </summary>
    public string CropName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the category of the land's product
    /// </summary>
    public string Category { get; set; } = null!;

    /// <summary>
    /// Gets or sets wether the land's product has a greenhouse
    /// </summary>
    public bool HasGreenhouse { get; set; } = false;

    /// <summary>
    /// Gets or sets the irrigation method of the land's product
    /// </summary>
    public string IrrigationMethod { get; set; } = null!;

    /// <summary>
    /// Gets or sets the irrigation count of the land's product
    /// </summary>
    public double IrrigationCount { get; set; }

    /// <summary>
    /// Gets or sets the irrigation source of the land's product
    /// </summary>
    public string IrrigationSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the irrigation water source of the land's product
    /// </summary>
    public string irrigationWaterSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the fertilizer of the land's product
    /// </summary>
    public string Fertilizer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the insecticide of the land's product
    /// </summary>
    public string Insecticide { get; set; } = null!;

    /// <summary>
    /// Gets or sets the insecticide of the land's product
    /// </summary>
    public string FundingType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the land of the land's product
    /// </summary>
    public Land Producer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the season that the land's product was produced in 
    /// </summary>
    public Season ProducedIn { get; set; } = null!;
}

