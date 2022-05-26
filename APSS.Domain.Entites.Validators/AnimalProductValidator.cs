using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="AnimalProduct"/>
/// </summary>
public sealed class AnimalProductValidator : Validator<AnimalProduct>
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