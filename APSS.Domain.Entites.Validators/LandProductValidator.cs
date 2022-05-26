using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="LandProduct"/>
/// </summary>
public sealed class LandProductValidator : Validator<LandProduct>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public LandProductValidator()
    {
        RuleFor(l => l.CropName)
            .NotEmpty()
            .WithMessage("name is required");

        RuleFor(l => l.Quantity)
            .GreaterThan(0)
            .WithMessage("quantity must be higher than 0");
    }
}