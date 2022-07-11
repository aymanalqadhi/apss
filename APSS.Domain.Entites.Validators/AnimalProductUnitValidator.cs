using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Account"/>
/// </summary>
public sealed class AnimalProductUnitValidator : Validator<AnimalProductUnit>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public AnimalProductUnitValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty()
            .WithMessage("animal product unit name cannot be empty");
    }
}