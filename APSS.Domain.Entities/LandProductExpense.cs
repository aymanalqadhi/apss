namespace APSS.Domain.Entities;

/// <summary>
/// A class to represt the expenses of the land's product
/// </summary>
public sealed class LandProductExpense : AuditableEntity
{
    /// <summary>
    /// Gets or sets the type of the expense
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Gets or sets the price of the expense
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the product that the expense was spent on
    /// </summary>
    public LandProduct SpentOn { get; set; } = null!;
}

