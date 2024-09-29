using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class Chairperson
    {
        [Key]
        public int chairpersonId { get; set; }
        [ForeignKey(nameof(Faculty))]
        public int facultyId { get; set; }

        //navigation properties
        public Faculty Faculty { get; set; }
    }
}
