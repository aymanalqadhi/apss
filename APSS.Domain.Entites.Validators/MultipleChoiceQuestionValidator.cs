using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="MultipleChoiceQuestion"/>
/// </summary>
public sealed class MultipleChoiceQuestionValidator : Validator<MultipleChoiceQuestion>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public MultipleChoiceQuestionValidator()
    {
        RuleFor(q => q.CandidateAnswers.Count)
            .GreaterThanOrEqualTo(2)
            .WithMessage("multiple choice questions must have at least two answers");

    }
}