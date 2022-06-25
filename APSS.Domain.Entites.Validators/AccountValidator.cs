using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Account"/>
/// </summary>
public sealed class AccountValidator : Validator<Account>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public AccountValidator()
    {
        RuleFor(u => u.HolderName)
            .NotEmpty()
            .WithMessage("account name cannot be empty");

        RuleFor(user => user.NationalId)
            .NotEmpty()
            .When(u => u.NationalId is not null)
            .WithMessage("account national ID cannot be empty");
    }
}