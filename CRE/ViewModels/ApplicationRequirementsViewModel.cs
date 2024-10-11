using CRE.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CRE.ViewModels
{
    public class ApplicationRequirementsViewModel
    {
        public User User { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationForms> EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLog { get; set; }
        public List<CoProponent> CoProponent { get; set; }
        public ReceiptInfo ReceiptInfo { get; set; }

        [Required(ErrorMessage = "File Upload for Ethics Form 9 Application for Ethics Review of New Protocol is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "Ethics Form 9")]
        public IFormFile Form9 { get; set; }

        [Required(ErrorMessage = "File Upload for Ethics Form 10 Research Study Protocol is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "Ethics Form 10")]
        public IFormFile Form10 { get; set; }

        [Required(ErrorMessage = "File Upload for Ethics Form 10.1 Non-Human Determination Form is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "Ethics Form 10.1")]
        public IFormFile Form10_1 { get; set; }

        [Required(ErrorMessage = "File Upload for Ethics Form Informed Consent 11 is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "Ethics Form 11")]
        public IFormFile Form11 { get; set; }

        [Required(ErrorMessage = "File Upload for Ethics Form 12 Assent Form is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "Ethics Form 12")]
        public IFormFile Form12 { get; set; }

        [Required(ErrorMessage = "File Upload for Ethics Form 15 Application for Ethics Review of Amendments is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "Ethics Form 15")]
        public IFormFile Form15 { get; set; }

        [Required(ErrorMessage = "File Upload for Ethics Form 18 Terminal Report is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "Ethics Form 18")]
        public IFormFile Form18 { get; set; }

        [Required(ErrorMessage = "File Upload for Co-Authorship Agreement is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "CAA")]
        public IFormFile CAA { get; set; }

        [Required(ErrorMessage = "File Upload for Researcher Curriculum Vitae is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "Researcher Curriculum Vitae")]
        public IFormFile RCV { get; set; }

        [Required(ErrorMessage = "File Upload for Certificate of Validity is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "CV")]
        public IFormFile CV { get; set; }

        [Required(ErrorMessage = "File Upload for Letter of Intent is required.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "pdf", ErrorMessage = "Please upload the file in PDF file format.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size must be less than 5MB.")]
        [Display(Name = "LI")]
        public IFormFile LI { get; set; }


    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"File size must be less than {_maxFileSize / (1024 * 1024)}MB.";
        }
    }
}
