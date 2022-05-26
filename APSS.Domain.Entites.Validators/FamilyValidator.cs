using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Family"/>
/// </summary>
public sealed class FamilyValidator : Validator<Family>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public FamilyValidator()
    {
        RuleFor(f => f.Name)
            .NotEmpty()
            .WithMessage("name is required");
    }
}