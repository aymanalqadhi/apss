using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="LogTag"/>
/// </summary>
public sealed class LogTagValidator : Validator<LogTag>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public LogTagValidator()
    {
        RuleFor(l => l.Value)
            .NotEmpty()
            .WithMessage("log tag value cannot be empty");
    }
}