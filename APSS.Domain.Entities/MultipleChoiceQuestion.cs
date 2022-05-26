namespace APSS.Domain.Entities;

public sealed class MultipleChoiceQuestion : Question
{
    /// <summary>
    /// Gets or sets the collection of possible answers to this question
    /// </summary>
    public ICollection<MultipleChoiceAnswer> Answers { get; set; } = null!;

    /// <summary>
    /// Gets or sets whether multiple answers can be selected at a time
    /// </summary>
    public bool CanMultiSelect { get; set; } = false;
}
    