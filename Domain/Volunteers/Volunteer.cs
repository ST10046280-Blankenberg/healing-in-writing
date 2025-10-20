using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Volunteers;

// TODO: Express volunteer identity and engagement details within the domain.
public class Volunteer
{
    // TODO: Include personal information, enrolment state, and role assignments.
    [Key]
    public int VolunteerId { get; set; }        // PK: volunteer_id

    [Required]
    public int UserId { get; set; }             // FK: user_id

    //TODO: Implement availability as system develops
    //public Boolean available { get; set; }

    // TODO: Add navigation properties for related entities
    //public User User { get; set; }
}
