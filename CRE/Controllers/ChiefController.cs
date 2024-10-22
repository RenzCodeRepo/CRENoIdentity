using CRE.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CRE.ViewModels;
using CRE.Services;
using CRE.Models;
namespace CRE.Controllers
{
    public class ChiefController : Controller
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

        public ChiefController(
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

        public async Task<IActionResult> Applications()
        {

            var viewModel = new InitialReviewListViewModel
            {
                PendingApplications = await _initialReviewServices.GetPendingApplicationsAsync(),
                ApprovedApplications = await _initialReviewServices.GetApprovedApplicationsAsync(),
                ReturnedApplications = await _initialReviewServices.GetReturnedApplicationsAsync(),
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Details(string urecNo)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return NotFound();
            }

            // Fetch the specific EthicsApplication based on UREC number
            var application = await _ethicsApplicationServices.GetApplicationByUrecNoAsync(urecNo);
            if (application == null)
            {
                return NotFound();
            }

            // Fetch related data
            var nonFundedResearchInfo = await _nonFundedResearchInfoServices.GetNonFundedResearchByUrecNoAsync(application.userId);
            var initialReview = await _initialReviewServices.GetInitialReviewByUrecNoAsync(urecNo);
            var ethicsApplicationForms = await _ethicsApplicationFormsServices.GetAllFormsByUrecAsync(urecNo);
            var ethicsApplicationLog = await _ethicsApplicationLogServices.GetLogsByUrecNoAsync(urecNo);

            // Create and populate the ViewModel with related data
            var viewModel = new AssignReviewTypeViewModel
            {
                EthicsApplication = new List<EthicsApplication> { application },
                
                InitialReview = initialReview,
                EthicsApplicationForms = ethicsApplicationForms,
                EthicsApplicationLog = ethicsApplicationLog,
                ReviewType = initialReview?.status ?? "Approved" 
            };

            // Pass the view model to the Details view
            return View(viewModel);
        }

    }
}
