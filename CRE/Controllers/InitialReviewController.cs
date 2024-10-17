using CRE.Interfaces;
using CRE.Services;
using CRE.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class InitialReviewController : Controller
    {
        private readonly IInitialReviewServices _initialReviewServices;

        public InitialReviewController(IInitialReviewServices initialReviewServices)
        {
            _initialReviewServices = initialReviewServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> InitialReview()
        {
            var viewModel = new InitialReviewListViewModel
            {
                PendingApplications = await _initialReviewServices.GetPendingApplicationsAsync(),
                ApprovedApplications = await _initialReviewServices.GetApprovedApplicationsAsync()
            };

            return View(viewModel);
        }

     
        [HttpPost]
        public async Task<IActionResult> ApproveApplication(string urecNo, string comments)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("Invalid UrecNo.");
            }

            await _initialReviewServices.ApproveApplicationAsync(urecNo, comments);

            return RedirectToAction("InitialReview");
        }

        [HttpPost]
        public async Task<IActionResult> ReturnApplication(string urecNo, string comments)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("Invalid UrecNo.");
            }

            await _initialReviewServices.ReturnApplicationAsync(urecNo, comments);

            return RedirectToAction("InitialReview");
        }


        public async Task<IActionResult> Details(string urecNo)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return NotFound();
            }

            try
            {
                // Fetch application details from the service layer
                var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo);
                return View(applicationDetails);
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., application not found)
                return NotFound(ex.Message);
            }
        }


    }
}
