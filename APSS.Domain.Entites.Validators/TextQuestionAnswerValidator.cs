using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="TextQuestionAnswer"/>
/// </summary>
public sealed class TextQuestionValidator : Validator<TextQuestionAnswer>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public TextQuestionValidator()
    {
        RuleFor(t => t.Answer)
            .Must(a => !string.IsNullOrEmpty(a?.Trim()))
            .When(a => a.Question.IsRequired)
            .WithMessage("a text answer is required");
    }
}