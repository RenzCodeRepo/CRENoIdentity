using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class Chief
    {
        [Key]
        public int chiefId { get; set; }
        [ForeignKey(nameof(User))]
        public int userId { get; set; }
        [Required]
        public string center { get; set; }



        //navigation properties
        public ICollection<InitialReview> InitialReview { get; set; } = new List<InitialReview>();
        public User User { get; set; }
    }
}
