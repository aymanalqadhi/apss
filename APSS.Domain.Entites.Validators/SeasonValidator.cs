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

        RuleFor(s => s.BeginDay)
            .GreaterThan(0)
            .LessThan(32)
            .WithMessage("begin day cannot be less than 1 or greater than 31");

        RuleFor(s => s.EndDay)
            .GreaterThan(0)
            .LessThan(32)
            .WithMessage("end day cannot be less than 1 or greater than 31");

        RuleFor(s => s.BeginMonth)
            .GreaterThan(0)
            .LessThan(13)
            .WithMessage("begin month cannot be less than 1 or greater than 12");

        RuleFor(s => s.EndMonth)
            .GreaterThan(0)
            .LessThan(13)
            .WithMessage("begin day cannot be less than 1 or greater than 12");
    }
}