using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Season"/>
/// </summary>
public sealed class SeasonValidator : Validator<Season>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public SeasonValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty()
            .WithMessage("name is required");
    }
}