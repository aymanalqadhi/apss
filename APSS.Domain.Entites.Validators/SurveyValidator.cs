using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Survey"/>
/// </summary>
public sealed class SurveyValidator : Validator<Survey>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public SurveyValidator()
    {
        RuleFor(survey => survey.Name)
            .NotEmpty()
            .WithMessage("survey name cannot be empty");

        RuleFor(s => s.CreatedBy.AccessLevel)
            .NotEqual(AccessLevel.Farmer);
    }
}

