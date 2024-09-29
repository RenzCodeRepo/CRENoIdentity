using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class Secretariat
    {
        [Key]
        public int secretariatId { get; set; }
        [ForeignKey(nameof(Faculty))]
        public int facultyId { get; set; }
        [Required]

        //navigation property
        public Faculty Faculty { get; set; }
        public ICollection<InitialReview> InitialReview { get; set; } = new List<InitialReview>();
    }
}
