using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.LandProductExpense"/>
/// </summary>
public sealed class LandProductExpenseValidator : Validator<Entities.LandProductExpense>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public LandProductExpenseValidator()
    {
        RuleFor(l => l.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("price must be higher than or equal to 0");

    }
}