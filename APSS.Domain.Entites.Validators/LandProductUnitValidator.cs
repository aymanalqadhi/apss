using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Account"/>
/// </summary>
public sealed class LandProductUnitValidator : Validator<LandProductUnit>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public LandProductUnitValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty()
            .WithMessage("land product unit name cannot be empty");
    }
}