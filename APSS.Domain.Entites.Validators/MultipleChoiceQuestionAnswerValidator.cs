using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="MultipleChoiceQuestion"/>
/// </summary>
public sealed class MultipleChoiceQuestionAnswerValidator : Validator<MultipleChoiceQuestionAnswer>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public MultipleChoiceQuestionAnswerValidator()
    {
        RuleFor(a => a.Question)
            .Must(q => q is MultipleChoiceQuestion)
            .WithMessage("invalid question type");

        RuleFor(a => a.Answers.Count)
            .GreaterThan(0)
            .When(a => a.Question.IsRequired)
            .WithMessage("this question is required");

        RuleFor(a => a.Answers.Count)
            .GreaterThanOrEqualTo(2)
            .When(a => a.Question is MultipleChoiceQuestion question && question.CanMultiSelect);
    }
}