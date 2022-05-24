using FluentValidation;

namespace APSS.Domain.Entites.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.User"/>
/// </summary>
public sealed class UserValidator : Validator<Entities.User>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public UserValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty()
            .WithMessage("name is required");

        RuleFor(user => user.NationalId)
            .NotEmpty()
            .WithMessage("national ID is required");

        RuleFor(user => user.IsSupervisor)
            .Equal(false)
            .When(user => user.AccessLevel == Entities.AccessLevel.Farmer);


    }
}