using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Voluntary"/>
/// </summary>
public sealed class VoluntaryValidator : Validator<Voluntary>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public VoluntaryValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .WithMessage("name is required");

        RuleFor(v => v.Type)
            .NotEmpty()
            .WithMessage("type is required");

        RuleFor(v => v.Field)
           .NotEmpty()
           .WithMessage("field is required");
    }
}