namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a log
/// </summary>
public sealed class Log : AuditableEntity
{
    /// <summary>
    /// Gets or sets the message of the log
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp of the log
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the tags of the log
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// Gets the parsed tags collection
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> ParseTags()
        => Tags.Split(',');
}
