using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="TextQuestionAnswer"/>
/// </summary>
public sealed class LogicalQuestionAnswerValidator : Validator<LogicalQuestionAnswer>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public LogicalQuestionAnswerValidator()
    {
        RuleFor(a => a.Question)
            .Must(q => q is LogicalQuestion)
            .WithMessage("invalid question type");

        RuleFor(a => a.Answer)
            .NotNull()
            .When(a => a.Question.IsRequired)
            .WithMessage("an answer is required");
    }
}