using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="AnimalProductExpens"/>
/// </summary>
public sealed class AnimalProductExpenseValidator : Validator<AnimalProductExpens>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public AnimalProductExpenseValidator()
    {
        RuleFor(a => a.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("price must be higher or equal to 0");

    }
}