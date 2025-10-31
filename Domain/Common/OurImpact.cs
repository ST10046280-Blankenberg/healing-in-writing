using System;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Common
{
    public class OurImpact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PeopleHelped { get; set; } = 0;

        [Required]
        public int WorkshopsHosted { get; set; } = 0;

        [Required]
        public int PartnerOrganisations { get; set; } = 0;

        [Required]
        public int CitiesReached { get; set; } = 0;

        [StringLength(200)]
        public string UpdatedBy { get; set; } = "System";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public byte[]? RowVersion { get; set; }
    }
}

