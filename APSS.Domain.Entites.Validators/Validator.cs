using FluentValidation;

namespace APSS.Domain.Entites.Validators;

/// <summary>
/// Base validator type
/// </summary>
/// <typeparam name="T">The type to be validated</typeparam>
public class Validator<T> : AbstractValidator<T>
{
}
