using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Stories;

// TODO: Model the Story aggregate with fields required for narratives and metadata.
public class Story
{
    // TODO: Add identifiers, content, author references, and status tracking.
    [Key]
    public int StoryId { get; set; }        // PK: story_id

    [Required]
    public int UserId { get; set; }         // FK: user_id

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Summary { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

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
