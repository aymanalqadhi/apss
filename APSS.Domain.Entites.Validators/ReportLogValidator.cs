using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="ReportLog"/>
/// </summary>
public sealed class ReportLogValidator : Validator<ReportLog>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public ReportLogValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage("name is required");

    }
}