using CRE.Interfaces;
using CRE.Models;
using CRE.Services;
using CRE.ViewModels;
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

        // Constructor to initialize the services
        public EthicsApplicationController(
            IEthicsApplicationServices ethicsApplicationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            IUserServices userServices,
            IReceiptInfoServices receiptInfoServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices,
            IConfiguration configuration, ICoProponentServices coProponentServices)
        {
            _ethicsApplicationServices = ethicsApplicationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _userServices = userServices;
            _receiptInfoServices = receiptInfoServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _configuration = configuration;
            _coProponentServices = coProponentServices;
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
        public async Task<IActionResult> SubmitApplication(ApplyEthicsViewModel model)
        {

            if (!int.TryParse(_configuration["DevelopmentUserId"], out int devUserId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View(model);
            }

            var userExists = await _userServices.UserExistsAsync(devUserId);
            if (!userExists)
            {
                ModelState.AddModelError("", "User ID does not exist.");
                return View(model);
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
                // Handling the uploaded file (receiptFile)
                if (model.receiptFile != null && model.receiptFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.receiptFile.CopyToAsync(memoryStream); //converting IFormFile datatype to byte
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
                    ModelState.AddModelError("receiptFile", "Please upload a scanned receipt.");
                    return View(model);
                }

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

        public async Task<IActionResult> UploadForms()
        {
            return View();
        }
    }
}