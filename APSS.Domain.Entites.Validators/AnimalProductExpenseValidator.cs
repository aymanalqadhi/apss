using FluentValidation;

namespace APSS.Domain.Entites.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.AnimalProductExpens"/>
/// </summary>
public sealed class AnimalProductExpenseValidator : Validator<Entities.AnimalProductExpens>
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