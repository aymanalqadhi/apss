using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="TextQuestionAnswer"/>
/// </summary>
public sealed class TextQuestionAnswerValidator : Validator<TextQuestionAnswer>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public TextQuestionAnswerValidator()
    {
        RuleFor(t => t.Question)
            .Must(q => q is TextQuestion)
            .WithMessage("invalid question type");

        RuleFor(t => t.Answer)
            .Must(a => !string.IsNullOrEmpty(a?.Trim()))
            .When(a => a.Question.IsRequired)
            .WithMessage("a text answer is required");
    }
}