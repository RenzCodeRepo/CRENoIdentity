﻿using CRE.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CRE.Data.Validations; // Ensure you have this using directive for IFormFile

namespace CRE.ViewModels
{
    public class UploadFormsViewModel
    {
        public EthicsApplication EthicsApplication { get; set; }
        public AppUser User { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationForms> EthicsApplicationForms { get; set; } // List for multiple forms
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLog { get; set; } // For new logs when uploading the forms
        public ReceiptInfo ReceiptInfo { get; set; } // To display the PDF receipt
        public List<CoProponent> CoProponent { get; set; } // To display other proponents 

        // Flags to determine if the study involves human subjects and minors
        public bool InvolvesHumanSubjects { get; set; }
        public bool InvolvesMinors { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Form 9 Application for Ethics Review of New Protocol: ")]
        public IFormFile FORM9 { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Form 10 Research Study Protocol: ")]
        public IFormFile FORM10 { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Researcher/s Curriculum Vitae: ")]
        public IFormFile RCV { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Certificate of Validity: ")]
        public IFormFile CV { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Co-Authorship Agreement: ")]
        public IFormFile CAA { get; set; }

        [Required(ErrorMessage = "Form Upload is Required.")]
        [Display(Name = "Letter of Intent: ")]
        public IFormFile LI { get; set; }

        // Conditional required fields based on involvement with human subjects and minors
        [RequiredIfHumanSubjects(ErrorMessage = "Form Upload is Required if human subjects are involved.")]
        [Display(Name = "Form 11 Informed Consent: ")]
        public IFormFile FORM11 { get; set; }

        [RequiredIfHumanSubjects(ErrorMessage = "Form Upload is Required if human subjects are involved.")]
        [Display(Name = "Form 12 Assent Form: ")]
        public IFormFile FORM12 { get; set; }

        [RequiredIfMinors(ErrorMessage = "Form Upload is Required if minors are involved.")]
        [Display(Name = "Form 10.1 Non-Human Determination Form: ")]
        public IFormFile FORM10_1 { get; set; }
    }
}