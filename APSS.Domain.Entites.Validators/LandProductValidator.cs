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
            .WithMessage("land product name cannot be empty");

        RuleFor(l => l.Quantity)
            .GreaterThan(0)
            .WithMessage("land product quantity must be higher than 0");

        RuleFor(l => l.HarvestEnd)
           .GreaterThan(l => l.HarvestStart)
           .WithMessage("land harvest end date must be higher than harvest start date");
    }
}