namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a question
/// </summary>
public abstract class Question : AuditableEntity
{
    /// <summary>
    /// Gets or sers the index of the question
    /// </summary>
    public uint Index { get; set; }

    /// <summary>
    /// Gets or sets the text of the question
    /// </summary>
    public string Text { get; set; } = null!;

    /// <summary>
    /// Gets or sets wether the question is required or not
    /// </summary>
    public bool IsRequired { get; set; } = false;
}