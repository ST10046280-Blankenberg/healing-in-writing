using HealingInWriting.Domain.Shared;
using HealingInWriting.Domain.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealingInWriting.Domain.Stories;

/// <summary>
/// Represents a story, including content, author, tags, and status metadata.
/// </summary>
public class Story
{
    /// <summary>
    /// Unique identifier for the story.
    /// </summary>
    [Key]
    public int StoryId { get; set; }

    /// <summary>
    /// Foreign key referencing the user who authored the story.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Tags associated with the story for categorization and search.
    /// </summary>
    public List<Tag> Tags { get; set; } = new();

    /// <summary>
    /// Title of the story.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    /// <summary>
    /// Short summary or abstract of the story.
    /// </summary>
    [StringLength(500)]
    public string Summary { get; set; }

    /// <summary>
    /// Main content or body of the story.
    /// </summary>
    [Required]
    public string Content { get; set; }

    /// <summary>
    /// Current lifecycle status of the story (e.g., Draft, Published).
    /// </summary>
    [Required]
    public StoryStatus Status { get; set; }

    /// <summary>
    /// Date and time when the story was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the story was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property for the author (user profile) of the story.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public UserProfile Author { get; set; }
}

/// <summary>
/// Defines the lifecycle status of a story.
/// </summary>
public enum StoryStatus
{
    /// <summary>
    /// Story is being drafted and not yet submitted.
    /// </summary>
    Draft,

    /// <summary>
    /// Story has been submitted for review.
    /// </summary>
    Submitted,

    /// <summary>
    /// Story has been approved and published.
    /// </summary>
    Published,

    /// <summary>
    /// Story has been rejected during review.
    /// </summary>
    Rejected,

    /// <summary>
    /// Story has been archived.
    /// </summary>
    Archived
}
