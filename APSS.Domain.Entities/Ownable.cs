namespace APSS.Domain.Entities;

public abstract class Ownable : Confirmable
{
    /// <summary>
    /// Gets or sets the name of the ownable item
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the owner of the ownable item
    /// </summary>
    public User OwnedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of expenses spent on this item
    /// </summary>
    public ICollection<ProductExpense> Expenses { get; set; } = new List<ProductExpense>();
}
