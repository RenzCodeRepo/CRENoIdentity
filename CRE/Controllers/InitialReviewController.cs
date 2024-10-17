using CRE.Interfaces;
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
            var viewModel = await _initialReviewServices.GetEthicsApplicationsForInitialReviewAsync();

            return View(viewModel);
        }
    }
}
