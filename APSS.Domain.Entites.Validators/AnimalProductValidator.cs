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
            .WithMessage("animal product name cannot be empty");

        RuleFor(a => a.Quantity)
            .GreaterThan(0)
            .WithMessage("animal product quantity must be higher than 0");
    }
}