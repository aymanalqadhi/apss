using APSS.Domain.Entities;

using FluentValidation;

namespace APSS.Domain.Entites.Validators;

/// <summary>
/// A validator for the entity <see cref="User"/>
/// </summary>
public sealed class UserValidator : Validator<User>
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

        RuleFor(u => u.SupervisedBy)
            .Equal(null as User)
            .When(u => u.AccessLevel == AccessLevel.Root);

        RuleFor(u => u.AccessLevel)
            .Equal(AccessLevel.Farmer)
            .When(u => u.SupervisedBy!.AccessLevel == AccessLevel.Group)
            .WithMessage("a farmer must be supervied by a group superviosr");
    }
}