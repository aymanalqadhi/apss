namespace APSS.Domain.Entities;

public sealed class TextQuestion : Question
{
    /// <summary>
    /// Gets or sets the plain text of the answer
    /// </summary>
    public string Answer { get; set; } = null!;
}
