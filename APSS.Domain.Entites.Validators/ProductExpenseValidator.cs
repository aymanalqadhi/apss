using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="ProductExpense"/>
/// </summary>
public sealed class ProductExpenseValidator : Validator<ProductExpense>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public ProductExpenseValidator()
    {
        RuleFor(a => a.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("price must be higher or equal to 0");

    }
}