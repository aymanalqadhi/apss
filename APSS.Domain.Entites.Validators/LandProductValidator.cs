using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.LandProduct"/>
/// </summary>
public sealed class LandProductValidator : Validator<Entities.LandProduct>
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
            .GreaterThanOrEqualTo(0)
            .WithMessage("quantity must be higher or equal to 0");

    }
}