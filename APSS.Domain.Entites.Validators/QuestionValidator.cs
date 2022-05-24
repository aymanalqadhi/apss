using FluentValidation;

namespace APSS.Domain.Entites.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.Question"/>
/// </summary>
public sealed class QuestionValidator : Validator<Entities.Question>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public QuestionValidator()
    {
        RuleFor(q => q.Index)
            .GreaterThanOrEqualTo(0)
            .WithMessage("index is required and cannot be less than 0");

    }
}