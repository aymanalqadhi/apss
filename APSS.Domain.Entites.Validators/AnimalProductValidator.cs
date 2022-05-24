using FluentValidation;

namespace APSS.Domain.Entites.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.AnimalProduct"/>
/// </summary>
public sealed class AnimalProductValidator : Validator<Entities.AnimalProduct>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public AnimalProductValidator()
    {
        RuleFor(a => a.Name)
            .NotEmpty()
            .WithMessage("name is required");

        RuleFor(a => a.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("quantity must be higher or equal to 0");

    }
}