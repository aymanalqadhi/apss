using FluentValidation;

namespace APSS.Domain.Entities.Validators;

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
        RuleFor(u => u.Name)
            .NotEmpty()
            .WithMessage("user name cannot be empty");

        RuleFor(user => user.NationalId)
            .NotEmpty()
            .WithMessage("user national ID cannot be empty");

        RuleFor(u => u.SupervisedBy)
            .NotNull()
            .When(u => u.AccessLevel != AccessLevel.Root)
            .WithMessage("a user must have a supervisor");

        RuleFor(u => u.SupervisedBy)
            .Null()
            .When(u => u.AccessLevel == AccessLevel.Root)
            .WithMessage("a root user cannot have a supervisor");

        RuleFor(u => u.AccessLevel)
            .Equal(AccessLevel.Farmer)
            .When(u => u.SupervisedBy!.AccessLevel == AccessLevel.Group)
            .WithMessage("a farmer must be supervied by a group superviosr");

        RuleFor(u => u.AccessLevel)
            .Equal(AccessLevel.Group)
            .When(u => u.SupervisedBy!.AccessLevel == AccessLevel.Village)
            .WithMessage("a group supervisor must be supervied by a village superviosr");

        RuleFor(u => u.AccessLevel)
            .Equal(AccessLevel.Village)
            .When(u => u.SupervisedBy!.AccessLevel == AccessLevel.District)
            .WithMessage("a village supervisor must be supervied by a district superviosr");

        RuleFor(u => u.AccessLevel)
            .Equal(AccessLevel.District)
            .When(u => u.SupervisedBy!.AccessLevel == AccessLevel.Directorate)
            .WithMessage("a district supervisor must be supervied by a directorate superviosr");

        RuleFor(u => u.AccessLevel)
            .Equal(AccessLevel.Directorate)
            .When(u => u.SupervisedBy!.AccessLevel == AccessLevel.Governorate)
            .WithMessage("a directorate superviosr must be supervied by a city superviosr");

        RuleFor(u => u.AccessLevel)
            .Equal(AccessLevel.Governorate)
            .When(u => u.SupervisedBy!.AccessLevel == AccessLevel.Presedint)
            .WithMessage("a city superviosr must be supervied by the presedint");

        RuleFor(u => u.AccessLevel)
            .Equal(AccessLevel.Presedint)
            .When(u => u.SupervisedBy!.AccessLevel == AccessLevel.Root)
            .WithMessage("a presedint must be supervied by a root");
    }
}