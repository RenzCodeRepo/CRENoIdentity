﻿using CRE.Interfaces;
using CRE.Models;
using CRE.Services;
using CRE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http; // For IFormFile and FormFile
using System.IO; // For MemoryStream
using System.Linq; // For LINQ methods like FirstOrDefault


namespace CRE.Controllers
{
    public class EthicsApplicationFormsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEthicsApplicationServices _ethicsApplicationServices;
        private readonly INonFundedResearchInfoServices _nonFundedResearchInfoServices;
        private readonly IAppUserServices _userServices;
        private readonly IReceiptInfoServices _receiptInfoServices;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly ICoProponentServices _coProponentServices;
        private readonly IEthicsApplicationFormsServices _ethicsApplicationFormsServices;
        private readonly IInitialReviewServices _initialReviewServices;

        public EthicsApplicationFormsController(
            IConfiguration configuration,
            IEthicsApplicationServices ethicsApplicationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            IAppUserServices userServices,
            IReceiptInfoServices receiptInfoServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices,
            ICoProponentServices coProponentServices,
            IEthicsApplicationFormsServices ethicsApplicationFormsServices,
            IInitialReviewServices initialReviewServices)
        {
            _configuration = configuration;
            _ethicsApplicationServices = ethicsApplicationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _userServices = userServices;
            _receiptInfoServices = receiptInfoServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _coProponentServices = coProponentServices;
            _ethicsApplicationFormsServices = ethicsApplicationFormsServices;
            _initialReviewServices = initialReviewServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDtsNo(string urecNo, string dtsNo)
        {
            // Validate DTS No. format
            var dtsPattern = @"^\d{4}-\d{4}-\d{2}$"; // Pattern: xxxx-xxxx-xx
            if (!Regex.IsMatch(dtsNo, dtsPattern))
            {
                ModelState.AddModelError("dtsNo", "DTS No. must be in the format xxxx-xxxx-xx.");
                return PartialView("_editDtsModal", new UploadFormsViewModel { EthicsApplication = new EthicsApplication { urecNo = urecNo, dtsNo = dtsNo } });
            }
            // Check for required fields
            if (string.IsNullOrEmpty(urecNo) || string.IsNullOrEmpty(dtsNo))
            {
                ModelState.AddModelError("", "DTS No. and Urec No. are required.");
                // Return the modal with the current state, including errors
                return PartialView("_editDtsModal", new UploadFormsViewModel { EthicsApplication = new EthicsApplication { urecNo = urecNo, dtsNo = dtsNo } });
            }

            // Find the ethics application by urecNo
            var ethicsApplication = await _ethicsApplicationServices.GetApplicationByUrecNoAsync(urecNo);
            if (ethicsApplication == null)
            {
                return NotFound(); // Handle case where application doesn't exist
            }

            // Check if the DTS number already exists
            var existingDts = await _ethicsApplicationServices.GetApplicationByDtsNoAsync(dtsNo);
            if (existingDts != null)
            {
                ModelState.AddModelError("dtsNo", "This DTS No. is already in use.");
                return PartialView("_editDtsModal", new UploadFormsViewModel { EthicsApplication = ethicsApplication });
            }

            // Update the DTS No.
            ethicsApplication.dtsNo = dtsNo;

            // Save the changes
            await _ethicsApplicationServices.SaveChangesAsync();

            return RedirectToAction("UploadForms", "EthicsApplicationForms", new { urecNo = urecNo });
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

            // Retrieve the logged-in user's ID from Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View("Error"); // Return an error view
            }

            // Fetch all necessary data
            var ethicsApplication = await _ethicsApplicationServices.GetApplicationByUrecNoAsync(urecNo);
            var nonFundedResearchInfo = await _nonFundedResearchInfoServices.GetNonFundedResearchByUrecNoAsync(urecNo);
            var receiptInfo = await _receiptInfoServices.GetReceiptInfoByUrecNoAsync(urecNo);
            var ethicsApplicationLogs = await _ethicsApplicationLogServices.GetLogsByUrecNoAsync(urecNo);
            var latestComment = await _ethicsApplicationLogServices.GetLatestCommentByUrecNoAsync(urecNo); // Fetch the latest comment
            var ethicsApplicationForms = await _ethicsApplicationFormsServices.GetAllFormsByUrecAsync(urecNo);
            var initialReview = await _initialReviewServices.GetInitialReviewByUrecNoAsync(urecNo); // Get InitialReview data
            var user = await _userServices.GetByIdAsync(userId); // Use Identity UserId (string)

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
                    Id = user.Id, // Use Identity user ID (string)
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
                LatestComment = latestComment, // Add the latest comment to the ViewModel
                CoProponent = coProponents.ToList(),
                InitialReview = initialReview// Ensure it's a List
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadForms(UploadFormsViewModel model)
        {
            // Retrieve the logged-in user's ID from Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View("Error"); // Return view with some error handling
            }

            // Retrieve existing forms for the specified application
            var existingForms = await _ethicsApplicationFormsServices.GetAllFormsByUrecAsync(model.EthicsApplication.urecNo);

            // If this is a re-upload, populate the existing forms in the model
            if (existingForms != null && existingForms.Any())
            {
                model.FORM9 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.ethicsFormId == "FORM9")?.file, "FORM9.pdf");
                model.FORM10 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.ethicsFormId == "FORM10")?.file, "FORM10.pdf");
                model.FORM10_1 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.ethicsFormId == "FORM10_1")?.file, "FORM10_1.pdf");
                model.FORM11 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.ethicsFormId == "FORM11")?.file, "FORM11.pdf");
                model.FORM12 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.ethicsFormId == "FORM12")?.file, "FORM12.pdf");
                model.CAA = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.ethicsFormId == "CAA")?.file, "CAA.pdf");
                model.RCV = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.ethicsFormId == "RCV")?.file, "RCV.pdf");
                model.CV = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.ethicsFormId == "CV")?.file, "CV.pdf");
                model.LI = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.ethicsFormId == "LI")?.file, "LI.pdf");

                // Clear validation errors for these properties
                ModelState.Remove(nameof(model.FORM9));
                ModelState.Remove(nameof(model.FORM10));
                ModelState.Remove(nameof(model.FORM10_1));
                ModelState.Remove(nameof(model.FORM11));
                ModelState.Remove(nameof(model.FORM12));
                ModelState.Remove(nameof(model.CAA));
                ModelState.Remove(nameof(model.RCV));
                ModelState.Remove(nameof(model.CV));
                ModelState.Remove(nameof(model.LI));
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
            ModelState.Remove("EthicsApplication.userId");
            ModelState.Remove("EthicsApplication.CompletionReport");
            ModelState.Remove("InitialReview");

            // Handle logic based on whether the research involves humans or minors
            if (model.InvolvesHumanSubjects)
            {
                ModelState.Remove(nameof(model.FORM10_1));

                if (model.InvolvesMinors)
                {
                    ModelState.Remove(nameof(model.FORM10_1));
                }
                else
                {
                    ModelState.Remove(nameof(model.FORM12));
                }
            }
            else
            {
                ModelState.Remove(nameof(model.FORM11));
                ModelState.Remove(nameof(model.FORM12));
            }

            if (!ModelState.IsValid)
            {
                return View("UploadForms", model);
            }

            // Prepare the list of forms in the view model
            var fileProperties = new List<(string PropertyName, IFormFile NewFile, IFormFile EditFile)>
            {
                (nameof(model.FORM9), model.FORM9, model.editFORM9),
                (nameof(model.FORM10), model.FORM10, model.editFORM10),
                (nameof(model.FORM10_1), model.FORM10_1, model.editFORM10_1),
                (nameof(model.FORM11), model.FORM11, model.editFORM11),
                (nameof(model.FORM12), model.FORM12, model.editFORM12),
                (nameof(model.CAA), model.CAA, model.editCAA),
                (nameof(model.RCV), model.RCV, model.editRCV),
                (nameof(model.CV), model.CV, model.editCV),
                (nameof(model.LI), model.LI, model.editLI)
            };

            // Loop through the list of uploaded files
            foreach (var (propertyName, newFile, editFile) in fileProperties)
            {
                // Use the edit form if it's populated, otherwise use the new file
                var fileToUpload = editFile != null && editFile.Length > 0 ? editFile : newFile;

                if (fileToUpload != null && fileToUpload.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await fileToUpload.CopyToAsync(memoryStream);

                        // Check if the form already exists in the database
                        var existingForm = await _ethicsApplicationFormsServices.GetFormByIdAndUrecNoAsync(propertyName, model.EthicsApplication.urecNo);

                        if (existingForm != null)
                        {
                            // Update the existing form with the new or edited file
                            existingForm.file = memoryStream.ToArray();
                            existingForm.dateUploaded = DateOnly.FromDateTime(DateTime.UtcNow);
                            existingForm.fileName = $"{propertyName}_{model.EthicsApplication.urecNo}.pdf";
                            await _ethicsApplicationFormsServices.UpdateFormAsync(existingForm);
                        }
                        else
                        {
                            // Create a new instance of the EthicsApplicationForms object
                            var ethicsApplicationForm = new EthicsApplicationForms
                            {
                                urecNo = model.EthicsApplication.urecNo,
                                ethicsFormId = propertyName,
                                dateUploaded = DateOnly.FromDateTime(DateTime.UtcNow),
                                file = memoryStream.ToArray(),
                                fileName = $"{propertyName}_{model.EthicsApplication.urecNo}.pdf"
                            };

                            // Save the new form to the database
                            await _ethicsApplicationFormsServices.AddFormAsync(ethicsApplicationForm);
                        }
                    }
                }
            }

            // Create a log entry for the forms upload
            var uploadFormLog = new EthicsApplicationLog
            {
                urecNo = model.EthicsApplication.urecNo,
                userId = userId,
                status = "Pending for Evaluation",
                changeDate = DateTime.Now
            };
            await _ethicsApplicationLogServices.AddLogAsync(uploadFormLog);

            // Update the InitialReview status to "Pending"
            var initialReview = await _initialReviewServices.GetInitialReviewByUrecNoAsync(model.EthicsApplication.urecNo);
            if (initialReview != null)
            {
                initialReview.status = "Pending";
                await _initialReviewServices.UpdateInitialReviewAsync(initialReview);
            }

            // Log that the user has uploaded their forms
            TempData["SuccessMessage"] = "Forms uploaded successfully!";

            // Redirect after processing
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

