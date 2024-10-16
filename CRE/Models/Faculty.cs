using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class Faculty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int facultyId { get; set; }
        [ForeignKey(nameof(User))]
        public string userId { get; set; }
        [Required]
        public string? userType { get; set; }
        [Required]
        public int? salaryGrade { get; set; }


        //navigation properties
        public AppUser User { get; set; }
        public Chairperson Chairperson { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
    }
}
