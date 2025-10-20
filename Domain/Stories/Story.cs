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