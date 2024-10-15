using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

namespace CRE.Controllers
{
    public class EthicsApplicationFormsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEthicsApplicationServices _ethicsApplicationServices;
        private readonly INonFundedResearchInfoServices _nonFundedResearchInfoServices;
        private readonly IUserServices _userServices;
        private readonly IReceiptInfoServices _receiptInfoServices;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly ICoProponentServices _coProponentServices;
        private readonly IEthicsApplicationFormsServices _ethicsApplicationFormsServices;

        public EthicsApplicationFormsController(
            IConfiguration configuration,
            IEthicsApplicationServices ethicsApplicationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            IUserServices userServices,
            IReceiptInfoServices receiptInfoServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices,
            ICoProponentServices coProponentServices,
            IEthicsApplicationFormsServices ethicsApplicationFormsServices)
        {
            _configuration = configuration;
            _ethicsApplicationServices = ethicsApplicationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _userServices = userServices;
            _receiptInfoServices = receiptInfoServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _coProponentServices = coProponentServices;
            _ethicsApplicationFormsServices = ethicsApplicationFormsServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UploadForms(string urecNo)
        {
            // Check if urecNo is provided
            if (string.IsNullOrEmpty(urecNo))
            {
                ModelState.AddModelError("", "UrecNo is missing.");
                return View("Error"); // Return an error view with appropriate error message
            }

            // Retrieve the development user ID from the configuration
            var devUserIdString = _configuration["DevelopmentUserId"];
            if (!int.TryParse(devUserIdString, out int devUserId))
            {
                ModelState.AddModelError("", "Invalid user ID in configuration.");
                return View("Error"); // Return an error view
            }

            // Fetch all necessary data
            var ethicsApplication = await _ethicsApplicationServices.GetApplicationByUrecNoAsync(urecNo);
            var nonFundedResearchInfo = await _nonFundedResearchInfoServices.GetNonFundedResearchByUrecNoAsync(urecNo);
            var receiptInfo = await _receiptInfoServices.GetReceiptInfoByUrecNoAsync(urecNo);
            var ethicsApplicationLogs = await _ethicsApplicationLogServices.GetLogsByUrecNoAsync(urecNo);
            var ethicsApplicationForms = await _ethicsApplicationFormsServices.GetAllFormsByUrecAsync(urecNo);
            var user = await _userServices.GetByIdAsync(devUserId);

            // Ensure all necessary data exists
            if (ethicsApplication == null || nonFundedResearchInfo == null || user == null)
            {
                ModelState.AddModelError("", "Application or user data is missing.");
                return View("Error"); // Return error view for missing data
            }

            // Retrieve co-proponents based on the research ID
            var coProponents = await _coProponentServices.GetCoProponentsByResearchIdAsync(nonFundedResearchInfo.nonFundedResearchId);

            // Populate ViewModel
            var model = new UploadFormsViewModel
            {
                User = new AppUser
                {
                    //userId = user.userId,
                    fName = user.fName,
                    mName = user.mName,
                    lName = user.lName,
                    type = user.type
                },
                EthicsApplication = ethicsApplication,
                NonFundedResearchInfo = nonFundedResearchInfo,
                ReceiptInfo = receiptInfo,
                EthicsApplicationForms = ethicsApplicationForms,
                EthicsApplicationLog = ethicsApplicationLogs,
                CoProponent = coProponents.ToList() // Ensure it's a List

            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UploadForms(UploadFormsViewModel model)
        {
            var devUserIdString = _configuration["DevelopmentUserId"]; // or get it from logged-in user context

            if (!int.TryParse(devUserIdString, out int devUserId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View(); // return view with some error handling
            }
            // Removed unnecessary properties from ModelState
            ModelState.Remove("User");
            ModelState.Remove("CoProponent");
            ModelState.Remove("ReceiptInfo");
            ModelState.Remove("EthicsApplicationLog");
            ModelState.Remove("NonFundedResearchInfo");
            ModelState.Remove("EthicsApplication.User");
            ModelState.Remove("EthicsApplication.urecNo");
            ModelState.Remove("EthicsApplication.ReceiptInfo");
            ModelState.Remove("EthicsApplication.InitialReview");
            ModelState.Remove("EthicsApplication.fieldOfStudy");
            ModelState.Remove("EthicsApplication.NonFundedResearchInfo");
            ModelState.Remove("EthicsApplicationForms");
            ModelState.Remove("EthicsApplication.EthicsClearance");
            ModelState.Remove("EthicsApplication.CompletionReport");

            // Handle logic based on whether the research involves humans or minors
            if (model.InvolvesHumanSubjects)  // Assuming you have a boolean field in your model
            {
                // If the research involves humans
                ModelState.Remove(nameof(model.FORM10_1)); // Remove Form 10.1 from validation

                if (model.InvolvesMinors)  // Assuming another boolean field
                {
                    // If research involves minors, also remove Form 10.1 but keep Form 12
                    ModelState.Remove(nameof(model.FORM10_1)); // Already removed above, but redundant check
                }
                else
                {
                    // If no minors are involved, remove Form 12 from validation
                    ModelState.Remove(nameof(model.FORM12));
                }
            }
            else
            {
                // If research does not involve humans, only Form 10.1 is valid
                ModelState.Remove(nameof(model.FORM11)); // Remove Form 11 from validation
                ModelState.Remove(nameof(model.FORM12)); // Remove Form 12 from validation
            }

            // Validate the model
            var fileProperties = new List<(string PropertyName, IFormFile File)>
            {
                (nameof(model.FORM9), model.FORM9),
                (nameof(model.FORM10), model.FORM10),
                (nameof(model.FORM10_1), model.FORM10_1),
                (nameof(model.FORM11), model.FORM11),
                (nameof(model.FORM12), model.FORM12),
                (nameof(model.CAA), model.CAA),
                (nameof(model.RCV), model.RCV),
                (nameof(model.CV), model.CV),
                (nameof(model.LI), model.LI)
            };

            if (!ModelState.IsValid)
            {
                return View("UploadForms", model);
            }

            // Loop through the list of uploaded files
            foreach (var (propertyName, file) in fileProperties)
            {
                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);

                        // Create a new instance of the EthicsApplicationForms object
                        var ethicsApplicationForm = new EthicsApplicationForms
                        {
                            urecNo = model.EthicsApplication.urecNo,
                            ethicsFormId = propertyName,
                            dateUploaded = DateOnly.FromDateTime(DateTime.UtcNow),
                            file = memoryStream.ToArray(),
                             // Generate and set the filename based on your format
                            fileName = $"{propertyName}_{model.EthicsApplication.urecNo}.pdf" // Assuming you want a PDF extension
                        };

                        // Save to database (example repository method)
                        await _ethicsApplicationFormsServices.AddFormAsync(ethicsApplicationForm);
                    }
                }
            }
            var uploadFormLog = new EthicsApplicationLog
            {
                urecNo = model.EthicsApplication.urecNo,
                //userId = devUserId,
                status = "Forms Uploaded",
                changeDate = DateTime.Now
            };
            await _ethicsApplicationLogServices.AddLogAsync(uploadFormLog);

            //log that the user has uploaded their forms
            TempData["SuccessMessage"] = "Forms uploaded successfully!";

            // After processing, redirect to a confirmation page or return the view
            return RedirectToAction("UploadForms", new { urecNo = model.EthicsApplication.urecNo });
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile(string formId, string urecNo)
        {
            // Fetch the form from the database based on formId and urecNo
            var form = await _ethicsApplicationFormsServices.GetFormByIdAndUrecNoAsync(formId, urecNo);

            if (form.file == null)
            {
                return NotFound(); 
            }
           
            // Return the file with the specified filename
            return File(form.file, "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string formId, string urecNo)
        {
            // Fetch the form from the database based on formId and urecNo
            var form = await _ethicsApplicationFormsServices.GetFormByIdAndUrecNoAsync(formId, urecNo);

            if (form.file == null)
            {
                return NotFound();
            }

            // Generate the filename based on the property name and urecNo
            var filename = $"{formId}_{urecNo}.pdf"; // Adjust this format as needed

            // Return the file for download
            return File(form.file, "application/pdf", filename);
        }

    }
}
