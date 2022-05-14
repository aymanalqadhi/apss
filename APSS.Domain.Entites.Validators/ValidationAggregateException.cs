using FluentValidation;
using FluentValidation.Results;

namespace BDP.Domain.Entities.Validators;

/// <summary>
/// An exception class to represent validation errors
/// </summary>
public class ValidationAggregateException : ValidationException
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="errors">The error list</param>
    public ValidationAggregateException(IEnumerable<ValidationFailure> errors) : base(errors)
    {
    }
}