using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ApplicationRequirements(ApplicationRequirementsViewModel model)
        {


            //// Removed unnecessary properties from ModelState
            //ModelState.Remove("User");
            //ModelState.Remove("User.type");
            //ModelState.Remove("User.Chief");
            //ModelState.Remove("User.fName");
            //ModelState.Remove("User.lName");
            //ModelState.Remove("User.mName");
            //ModelState.Remove("User.Faculty");
            //ModelState.Remove("CoProponent");
            //ModelState.Remove("ReceiptInfo");
            //ModelState.Remove("ReceiptInfo.urecNo");
            //ModelState.Remove("ReceiptInfo.receiptNo");
            //ModelState.Remove("ReceiptInfo.amountPaid");
            //ModelState.Remove("ReceiptInfo.scanReceipt");
            //ModelState.Remove("ReceiptInfo.EthicsApplication");
            //ModelState.Remove("EthicsApplication");
            //ModelState.Remove("NonFundedResearchInfo");
            //ModelState.Remove("NonFundedResearchInfo.User");
            //ModelState.Remove("NonFundedResearchInfo.title");
            //ModelState.Remove("NonFundedResearchInfo.campus");
            //ModelState.Remove("NonFundedResearchInfo.urecNo");
            //ModelState.Remove("NonFundedResearchInfo.college");
            //ModelState.Remove("NonFundedResearchInfo.university");
            //ModelState.Remove("NonFundedResearchInfo.EthicsClearance");
            //ModelState.Remove("NonFundedResearchInfo.EthicsApplication");
            //ModelState.Remove("NonFundedResearchInfo.nonFundedResearchId");
            //ModelState.Remove("NonFundedResearchInfo.CompletionCertificate");
            //ModelState.Remove("EthicsApplication.User");
            //ModelState.Remove("EthicsApplication.urecNo");
            //ModelState.Remove("EthicsApplication.ReceiptInfo");
            //ModelState.Remove("EthicsApplication.InitialReview");
            //ModelState.Remove("EthicsApplication.EthicsClearance");
            //ModelState.Remove("EthicsApplication.CompletionReport");
            //ModelState.Remove("EthicsApplication.fieldOfStudy");
            //ModelState.Remove("EthicsApplication.NonFundedResearchInfo");
            //ModelState.Remove("EthicsApplication.NonFundedResearchInfo");

            // Validate the model
            var fileProperties = new List<(string PropertyName, IFormFile File)>
            {
                (nameof(model.Form9), model.Form9),
                (nameof(model.Form10), model.Form10),
                (nameof(model.Form10_1), model.Form10_1),
                (nameof(model.Form11), model.Form11),
                (nameof(model.Form12), model.Form12),
                (nameof(model.Form15), model.Form15),
                (nameof(model.Form18), model.Form18),
                (nameof(model.CAA), model.CAA),
                (nameof(model.RCV), model.RCV),
                (nameof(model.CV), model.CV),
                (nameof(model.LI), model.LI)
            };


            if (!ModelState.IsValid)
            {
                return View("ApplicationRequirements", model);
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
                            urecNo = model.EthicsApplication.urecNo, // Assuming UrecNo is passed along
                            ethicsFormId = propertyName,
                            dateUploaded = DateOnly.FromDateTime(DateTime.UtcNow),
                            file = memoryStream.ToArray() // Store the file as byte array
                        };

                        // Save to database (example repository method)
                        await _ethicsApplicationFormsServices.AddFormAsync(ethicsApplicationForm);
                    }
                }
            }

            // After processing, redirect to a confirmation page or return the view
            return RedirectToAction("ApplicationRequirements", new { urecNo = model.EthicsApplication.urecNo });
        }
    }
}
