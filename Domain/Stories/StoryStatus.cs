namespace HealingInWriting.Domain.Stories;

/// <summary>
/// Defines the lifecycle status of a story.
/// </summary>
public enum StoryStatus
{
    /// <summary>
    /// Story is being drafted and not yet submitted
    /// </summary>
    Draft,

    /// <summary>
    /// Story has been submitted for review
    /// </summary>
    Submitted,

    /// <summary>
    /// Story has been approved and published
    /// </summary>
    Published,

    /// <summary>
    /// Story has been rejected during review
    /// </summary>
    Rejected,

    /// <summary>
    /// Story has been archived
    /// </summary>
    Archived
}
