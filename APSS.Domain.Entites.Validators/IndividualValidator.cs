using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.Individual"/>
/// </summary>
public sealed class IndividualValidator : Validator<Entities.Individual>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public IndividualValidator()
    {
        RuleFor(i => i.Name)
            .NotEmpty()
            .WithMessage("name is required");

    }
}