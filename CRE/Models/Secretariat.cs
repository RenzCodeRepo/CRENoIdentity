using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
        public class Secretariat
        {
            [Key]
            public int secretariatId { get; set; }
            [ForeignKey(nameof(AppUser))]
            public string userId { get; set; }
            [Required]

        //navigation property
        public AppUser AppUser { get; set; }
    }
}
