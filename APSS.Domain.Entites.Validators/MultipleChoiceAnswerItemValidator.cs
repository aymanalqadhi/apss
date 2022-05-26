using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="MultipleChoiceAnswerItem"/>
/// </summary>
public sealed class MultipleChoiceAnswerItemValidator : Validator<MultipleChoiceAnswerItem>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public MultipleChoiceAnswerItemValidator()
    {
        RuleFor(m => m.Value)
            .NotEmpty()
            .WithMessage("multiple choice answer item value cannot be empty");
    }
}