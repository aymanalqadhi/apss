﻿using FluentValidation;

namespace APSS.Domain.Entites.Validators;

/// <summary>
/// A validator for the entity <see cref="Entities.Survey"/>
/// </summary>
public sealed class SurveyValidator : Validator<Entities.Survey>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public SurveyValidator()
    {
        RuleFor(survey => survey.Name)
            .NotEmpty()
            .WithMessage("survey name cannot be empty");

    }
}

