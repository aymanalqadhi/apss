namespace APSS.Domain.Entities;

public sealed class MultipleChoiceAnswerItem : AuditableEntity
{
    /// <summary>
    /// Gets or sets the value of the answer
    /// </summary>
    public string Value { get; set; } = null!;
}