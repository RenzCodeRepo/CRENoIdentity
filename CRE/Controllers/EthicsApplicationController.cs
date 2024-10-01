using CRE.Interfaces;
using CRE.Models;
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

        // Constructor to initialize the services
        public EthicsApplicationController(
            IEthicsApplicationServices ethicsApplicationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            IUserServices userServices,
            IReceiptInfoServices receiptInfoServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices,
            IConfiguration configuration)
        {
            _ethicsApplicationServices = ethicsApplicationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _userServices = userServices;
            _receiptInfoServices = receiptInfoServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Applications()
        {
            // Return the view for the application form
            return View();
        }

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

                        EthicsApplication = new EthicsApplication(), // Empty for manual input
                        NonFundedResearchInfo = new NonFundedResearchInfo(), // Empty for manual input
                        ReceiptInfo = new ReceiptInfo(), // Empty for manual input

                        CoProponent = new List<CoProponent>() // Start with an empty list
                    };

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
    }

}

