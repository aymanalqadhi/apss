using APSS.Domain.Entities;

using FluentValidation;

namespace APSS.Domain.Entities.Validators;

/// <summary>
/// A validator for the entity <see cref="Skill"/>
/// </summary>
public sealed class SkillValidator : Validator<Skill>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public SkillValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty()
            .WithMessage("name is required");
    }
}