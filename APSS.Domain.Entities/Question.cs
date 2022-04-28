namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a question
/// </summary>
public sealed class Question : AuditableEntity
{
    /// <summary>
    /// Gets or sers the index of the question
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the type of the question
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Gets or sets the text of the question
    /// </summary>
    public string Text { get; set; } = null!;

    /// <summary>
    /// Gets or sets wether the question is required or not
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Gets or sets the survey that the question belongs to
    /// </summary>
    public Survey BelongsTo { get; set; } = null!;
}

