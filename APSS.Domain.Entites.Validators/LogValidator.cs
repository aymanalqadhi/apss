using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Log"/>
/// </summary>
public sealed class LogValidator : Validator<Log>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public LogValidator()
    {
        RuleFor(l => l.Message)
            .NotEmpty()
            .WithMessage("log message cannot be empty");
    }
}