using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Land"/>
/// </summary>
public sealed class LandValidator : Validator<Land>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public LandValidator()
    {
        RuleFor(l => l.Name)
            .NotEmpty()
            .WithMessage("land name cannot be empty");

        RuleFor(l => l.OwnedBy.AccessLevel)
            .Equal(AccessLevel.Farmer)
            .WithMessage("a land must be owned by a farmer");

        RuleFor(l => l.Latitude)
            .GreaterThanOrEqualTo(-90)
            .LessThanOrEqualTo(90)
            .WithMessage("latitude must be in the -90 and 90 range");

        RuleFor(l => l.Longitude)
           .GreaterThanOrEqualTo(-180)
           .LessThanOrEqualTo(180)
           .WithMessage("longitude must be in the -180 and 180 range");
    }
}