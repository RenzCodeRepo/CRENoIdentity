﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRE.Models
{
    public class CoProponent
    {
        [Key]
        public int coProponentId { get; set; }
        [ForeignKey(nameof(NonFundedResearchInfo))]
        public string nonFundedResearchId { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; } // Navigation property

        [Required(ErrorMessage ="Name is Required")]
        public string coProponentName { get; set; }
        [EmailAddress(ErrorMessage ="Invalid email format.")]
        public string? coProponentEmail { get; set; }
    }
}
