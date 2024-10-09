using CRE.Interfaces;
using CRE.Models;
using CRE.Services;
using CRE.ViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRE.Controllers
{
    public class EthicsApplicationController : Controller
    {
        private readonly IEthicsApplicationServices _ethicsApplicationServices;
        private readonly INonFundedResearchInfoServices _nonFundedResearchInfoServices;
        private readonly IUserServices _userServices;
        private readonly IReceiptInfoServices _receiptInfoServices;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly IConfiguration _configuration;
        private readonly ICoProponentServices _coProponentServices;
        private readonly IEthicsApplicationFormsServices _ethicsApplicationFormsServices;

        // Helper method for email validation
        private bool IsValidEmail(string email)
        {
            // Basic email validation logic here
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }
        // Constructor to initialize the services
        public EthicsApplicationController(
            IEthicsApplicationServices ethicsApplicationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            IUserServices userServices,
            IReceiptInfoServices receiptInfoServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices,
            IConfiguration configuration, ICoProponentServices coProponentServices,
            IEthicsApplicationFormsServices ethicsApplicationFormsServices)
        {
            _ethicsApplicationServices = ethicsApplicationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _userServices = userServices;
            _receiptInfoServices = receiptInfoServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _configuration = configuration;
            _coProponentServices = coProponentServices;
            _ethicsApplicationFormsServices = ethicsApplicationFormsServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ApplicationsAsync()
        {
            var devUserIdString = _configuration["DevelopmentUserId"]; // or get it from logged-in user context

            if (!int.TryParse(devUserIdString, out int devUserId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View(); // return view with some error handling
            }


            // Fetch the user's ethics applications
            var ethicsApplications = await _ethicsApplicationServices.GetApplicationsByUserAsync(devUserId);

            // Fetch the related NonFundedResearchInfo
            var nonFundedResearchInfos = await _nonFundedResearchInfoServices.GetNonFundedResearchByUserAsync(devUserId);

            var ethicsApplicationIds = ethicsApplications.Select(a => a.urecNo).ToList(); // Assuming Id is the primary key

            // Fetch the latest status from the logs for each application
            var ethicsApplicationLogs = await _ethicsApplicationLogServices.GetLatestLogsByApplicationIdsAsync(ethicsApplicationIds);

            // Populate the ApplicationsViewModel
            var model = new ApplicationsViewModel
            {
                EthicsApplication = ethicsApplications,
                NonFundedResearchInfo = nonFundedResearchInfos,
                EthicsApplicationLog = ethicsApplicationLogs
            };


            // Return the view for the application form
            return View(model);
        }
        //View Logic
        public async Task<IActionResult> SubmitApplication()
        {
            // Retrieve the development user ID from configuration
            var devUserIdString = _configuration["DevelopmentUserId"];

            if (int.TryParse(devUserIdString, out int devUserId))
            {
                // Fetch the user record from the service asynchronously
                var user = await _userServices.GetByIdAsync(devUserId);
                if (user != null)
                {
                    // Create an instance of the ViewModel
                    var model = new ApplyEthicsViewModel
                    {
                        User = new User
                        {
                            userId = user.userId,
                            fName = user.fName,
                            mName = user.mName,
                            lName = user.lName,
                            type = user.type
                        },
                        EthicsApplication = new EthicsApplication
                        {
                            userId = user.userId
                        },
                        EthicsApplicationLog = new List<EthicsApplicationLog>(),
                        NonFundedResearchInfo = new NonFundedResearchInfo
                        {
                            userId = user.userId
                        },
                        CoProponent = new List<CoProponent>() // Start with an empty list
                    };

                    // Only set ReceiptInfo for external users
                    if (user.type == "external")
                    {
                        model.ReceiptInfo = new ReceiptInfo(); // Create a new instance if external
                    }
                    else
                    {
                        model.ReceiptInfo = null; // Ensure it is null for internal users
                    }

                    // Pass the model to the view
                    return View(model);
                }
            }

            // If user not found, create an empty model
            var emptyModel = new ApplyEthicsViewModel
            {
                CoProponent = new List<CoProponent>()
            };

            return View(emptyModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitApplication(ApplyEthicsViewModel model)
        {
            if (model.CoProponent == null)
            {
                model.CoProponent = new List<CoProponent>();
            }
            if (!int.TryParse(_configuration["DevelopmentUserId"], out int devUserId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View(model);
            }

            var user = await _userServices.GetByIdAsync(devUserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User ID does not exist.");
                return View(model);
            }

            var userExists = await _userServices.UserExistsAsync(devUserId);
            if (!userExists)
            {
                ModelState.AddModelError("", "User ID does not exist.");
                return View(model);
            }
            model.User = new User
            {
                userId = user.userId,
                fName = user.fName,
                mName = user.mName,
                lName = user.lName,
                type = user.type
            };


            // Remove validation for fields that are not yet populated
            if (user.type == "internal")
            {
                // If internal users don't need a receipt, remove validation errors related to ReceiptInfo
                ModelState.Remove("ReceiptInfo.receiptNo");
                ModelState.Remove("ReceiptInfo.amountPaid");
                ModelState.Remove("receiptFile");
            }



            var ethicsApplication = model.EthicsApplication;
            ethicsApplication.urecNo = await _ethicsApplicationServices.GenerateUrecNoAsync();
            ethicsApplication.userId = devUserId;
            ethicsApplication.submissionDate = DateOnly.FromDateTime(DateTime.Now);

            var nonFundedResearchInfo = model.NonFundedResearchInfo;
            nonFundedResearchInfo.nonFundedResearchId = await _nonFundedResearchInfoServices.GenerateNonFundedResearchIdAsync();
            nonFundedResearchInfo.userId = devUserId;
            nonFundedResearchInfo.urecNo = ethicsApplication.urecNo;
            nonFundedResearchInfo.dateSubmitted = DateTime.Now;


            var ethicsApplicationLog = new EthicsApplicationLog
            {
                urecNo = ethicsApplication.urecNo,
                userId = devUserId,
                status = "Submitted",
                changeDate = DateTime.Now
            };

            // Removed some fields are to be inputed later
            ModelState.Remove("User");
            ModelState.Remove("ReceiptInfo");
            ModelState.Remove("EthicsApplication.User");
            ModelState.Remove("EthicsApplication.urecNo");
            ModelState.Remove("EthicsApplication.ReceiptInfo");
            ModelState.Remove("EthicsApplication.InitialReview");
            ModelState.Remove("EthicsApplication.EthicsClearance");
            ModelState.Remove("EthicsApplication.CompletionReport");
            ModelState.Remove("EthicsApplication.NonFundedResearchInfo");
            ModelState.Remove("NonFundedResearchInfo.User");
            ModelState.Remove("NonFundedResearchInfo.urecNo");
            ModelState.Remove("NonFundedResearchInfo.EthicsClearance");
            ModelState.Remove("NonFundedResearchInfo.EthicsApplication");
            ModelState.Remove("NonFundedResearchInfo.nonFundedResearchId");
            ModelState.Remove("NonFundedResearchInfo.CompletionCertificate");
            ModelState.Remove("ReceiptInfo.urecNo");
            ModelState.Remove("ReceiptInfo.scanReceipt");
            ModelState.Remove("ReceiptInfo.EthicsApplication");
            // Clear existing ModelState errors for CoProponents
            for (int i = 0; i < model.CoProponent.Count; i++)
            {
                ModelState.Remove($"CoProponent[{i}].NonFundedResearchInfo");
                ModelState.Remove($"CoProponent[{i}].nonFundedResearchId");
            }
            // Call the service method to check for a duplicate title
            var existingResearch = await _nonFundedResearchInfoServices.SearchByTitleAsync(model.NonFundedResearchInfo.title);

            // If a title match is found, add an error to the ModelState
            if (existingResearch != null)
            {
                ModelState.AddModelError("NonFundedResearchInfo.title", "This title has already been used for another ethics application.");
                return View(model);
            }
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {

                // If model state is invalid, return the view with the validation messages
                return View(model);
            }


            try
            {
                await _ethicsApplicationServices.ApplyForEthicsAsync(ethicsApplication);
                await _nonFundedResearchInfoServices.AddNonFundedResearchAsync(nonFundedResearchInfo);
                await _ethicsApplicationLogServices.AddLogAsync(ethicsApplicationLog);

                if (model.CoProponent != null && model.CoProponent.Any())
                {
                    foreach (var coProponent in model.CoProponent)
                    {
                        if (coProponent != null)
                        {
                            coProponent.nonFundedResearchId = nonFundedResearchInfo.nonFundedResearchId;
                            await _coProponentServices.AddCoProponentAsync(coProponent);
                        }
                    }



                }
                // Check if the user is internal
                if (model.User != null && model.User.type == "internal")
                {
                    // Internal users do not need to upload a receipt
                    TempData["SuccessMessage"] = "Your application has been submitted successfully (no receipt required).";
                    return RedirectToAction("Applications");
                }

                // For external users, handle the receipt file upload
                if (model.User != null && model.User.type == "external")
                {
                    // Handling the uploaded file (receiptFile)
                    if (model.receiptFile != null && model.receiptFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.receiptFile.CopyToAsync(memoryStream); // Converting IFormFile to byte[]
                            model.ReceiptInfo.scanReceipt = memoryStream.ToArray();
                        }

                        // Save ReceiptInfo if needed
                        if (model.ReceiptInfo != null)
                        {
                            model.ReceiptInfo.urecNo = ethicsApplication.urecNo;
                            await _receiptInfoServices.AddReceiptInfoAsync(model.ReceiptInfo);
                        }
                    }
                    else
                    {
                        // If no file is uploaded for an external user, show error
                        ModelState.AddModelError("receiptFile", "Please upload a scanned receipt.");
                        return View(model); // Return to the view with validation error
                    }
                }

                // If everything succeeds, show success message
                TempData["SuccessMessage"] = "Your application has been submitted successfully.";
                return RedirectToAction("Applications");
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", "There was an error saving your application. Please try again.");
                Console.WriteLine($"Exception: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ApplicationRequirements(string urecNo)
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
            var model = new ApplicationRequirementsViewModel
            {
                User = new User
                {
                    userId = user.userId,
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

            return View(model); // Pass the populated model to the view
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDtsNo(string urecNo, string dtsNo)
        {
            if (string.IsNullOrEmpty(urecNo) || string.IsNullOrEmpty(dtsNo))
            {
                ModelState.AddModelError("", "DTS No. and Urec No. are required.");
                return View("Error"); // Show an error view or handle appropriately
            }

            // Find the ethics application by urecNo
            var ethicsApplication = await _ethicsApplicationServices.GetApplicationByUrecNoAsync(urecNo);
            if (ethicsApplication == null)
            {
                return NotFound(); // Handle case where application doesn't exist
            }

            // Update the DTS No.
            ethicsApplication.dtsNo = dtsNo;

            // Save the changes (assuming _ethicsApplicationServices.SaveChangesAsync is available)
            await _ethicsApplicationServices.SaveChangesAsync();

            return RedirectToAction("ApplicationRequirements", new { urecNo = urecNo }); // Redirect to the same page to reflect the updated DTS No.
        }

    }
}