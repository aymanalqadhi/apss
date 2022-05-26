namespace APSS.Domain.Entities;

public sealed class LogicalQuestion : Question
{
    /// <summary>
    /// Gets or sets whther this question was answered by yes/true or no/false
    /// </summary>
    public bool Answer { get; set; }
}
