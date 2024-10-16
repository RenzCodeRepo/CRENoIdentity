﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class AppUser : IdentityUser
    {

        [Required, StringLength(30)]
        public string fName { get; set; }

        [Required, StringLength(30)]
        public string mName { get; set; }
        [Required, StringLength(30)]
        public string lName { get; set; }
        public string type { get; set; } //internal and external

        //navigation properties
        public Faculty? Faculty { get; set; }
        public Chief? Chief { get; set; }
        public Secretariat? Secretariat { get; set; }
        public ICollection<InitialReview> InitialReview { get; set; } = new List<InitialReview>();
        public ICollection<EthicsApplication> EthicsApplication { get; set; } = new List<EthicsApplication>();
        public ICollection<EthicsApplicationLog> EthicsApplicationLog { get; set; } = new List<EthicsApplicationLog>();
        public ICollection<NonFundedResearchInfo> NonFundedResearchInfo { get; set; } = new List<NonFundedResearchInfo>(); // Collection of research projects
        public ICollection<EthicsReport> EthicsReport { get; set;} = new List<EthicsReport>();
    }
}
