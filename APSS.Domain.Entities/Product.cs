namespace APSS.Domain.Entities;

public abstract class Product : Confirmable
{
    /// <summary>
    /// Gets or sets the expenses spent on this product
    /// </summary>
    public ICollection<ProductExpense> Expenses { get; set; } = new List<ProductExpense>();
}
