using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }

        [Required, StringLength(30)]
        public string fName { get; set; }

        [Required, StringLength(30)]
        public string mName { get; set; }
        [Required, StringLength(30)]
        public string lName { get; set; }
        public string type { get; set; } //internal and external

        //navigation properties
        public Faculty Faculty { get; set; }
        public Chief Chief { get; set; }
        public ICollection<EthicsApplication> EthicsApplication { get; set; } = new List<EthicsApplication>();
        public ICollection<EthicsApplicationLog> EthicsApplicationLog { get; set; } = new List<EthicsApplicationLog>();
        public ICollection<NonFundedResearchInfo> NonFundedResearchInfo { get; set; } = new List<NonFundedResearchInfo>(); // Collection of research projects
    }
}
