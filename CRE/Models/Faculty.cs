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
        public int userId { get; set; }
        [Required]
        public string userType { get; set; }
        [Required]
        public int salaryGrade { get; set; }


        //navigation properties
        public User User { get; set; }
        public Secretariat Secretariat { get; set; }
        public Chairperson Chairperson { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
    }
}
