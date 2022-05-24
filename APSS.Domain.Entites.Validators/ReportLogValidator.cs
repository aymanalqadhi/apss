using FluentValidation;

namespace APSS.Domain.Entites.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.ReportLog"/>
/// </summary>
public sealed class ReportLogValidator : Validator<Entities.ReportLog>
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