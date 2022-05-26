using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="AnimalGroup"/>
/// </summary>
public sealed class AnimalGroupValidator : Validator<AnimalGroup>
{
    /// <summary>
    /// Default constuctor
    /// </summary>
    public AnimalGroupValidator()
    {
        RuleFor(g => g.Name)
            .NotEmpty()
            .WithMessage("animal group name cannot be empty");

        RuleFor(g => g.Type)
            .NotEmpty()
            .WithMessage("animal group type cannot be empty");

        RuleFor(g => g.Quantity)
            .GreaterThan(0)
            .WithMessage("animal group quantity must be greater then 0");

        RuleFor(g => g.OwnedBy.AccessLevel)
            .Equal(AccessLevel.Farmer);

    }
}