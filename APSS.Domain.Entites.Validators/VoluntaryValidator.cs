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
            .WithMessage("voluntary name cannot be empty");

        RuleFor(v => v.Field)
           .NotEmpty()
           .WithMessage("voluntary field cannot be empty");
    }
}