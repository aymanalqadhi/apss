using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Individual"/>
/// </summary>
public sealed class IndividualValidator : Validator<Individual>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public IndividualValidator()
    {
        RuleFor(i => i.Name)
            .NotEmpty()
            .WithMessage("name is required");

        RuleFor(i => i.AddedBy.AccessLevel)
            .Equal(AccessLevel.Group)
            .WithMessage("only a group supervisor can add an individual");

    }
}