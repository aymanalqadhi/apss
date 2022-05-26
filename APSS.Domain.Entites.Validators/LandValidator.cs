using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.Land"/>
/// </summary>
public sealed class LandValidator : Validator<Entities.Land>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public LandValidator()
    {
        RuleFor(l => l.Name)
            .NotEmpty()
            .WithMessage("name is required");

        RuleFor(l => l.OwnedBy.AccessLevel)
            .Equal(Entities.AccessLevel.Farmer)
            .WithMessage("a land must be owned by a farmer");

    }
}