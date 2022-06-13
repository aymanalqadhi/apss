namespace APSS.Domain.Repositories.Exceptions;

public sealed class NotFoundException : Exception
{
    /// <inheritdoc/>
    public NotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}