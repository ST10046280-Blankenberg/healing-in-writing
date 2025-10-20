using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Shared
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

    }
}
