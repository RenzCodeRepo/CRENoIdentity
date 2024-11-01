using CRE.Data;
using CRE.Interfaces;
using CRE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRE.Controllers
{
    public class EthicsEvaluationController : Controller
    {
        private readonly IEthicsEvaluationServices _ethicsEvaluationServices;
        private readonly ApplicationDbContext _context;

        public EthicsEvaluationController(IEthicsEvaluationServices ethicsEvaluationService, ApplicationDbContext context)
        {
            _context = context;
            _ethicsEvaluationServices = ethicsEvaluationService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Results(string urecNo)
        {
            // Retrieve EthicsApplication along with its related evaluations
            var application = await _context.EthicsApplication
                .Include(e => e.User)
                .Include(e => e.EthicsEvaluation) // Retrieve all evaluations for this application
                    .ThenInclude(ev => ev.EthicsEvaluator) // Nested include for evaluator details
                        .ThenInclude(f => f.Faculty)
                            .ThenInclude(u => u.User)
                .Include(e => e.EthicsApplicationLog)
                .Include(e => e.EthicsApplicationForms)
                .FirstOrDefaultAsync(a => a.urecNo == urecNo);

            if (application == null)
            {
                return NotFound();
            }

            // Populate the view model with data
            var viewModel = new EvaluationResultsViewModel
            {
                AppUser = application.User,
                EthicsApplication = application,
                ApplicationLogs = application.EthicsApplicationLog,
                ApplicationForms = application.EthicsApplicationForms,
                EthicsEvaluation = application.EthicsEvaluation.ToList() // Ensure this is a list of evaluations
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AllApplications()
        {
            // Retrieve all applications along with their related evaluations and chief details
            var applications = await _context.EthicsApplication
                .Include(a => a.User)
                .Include(a => a.InitialReview) // Include InitialReview to access ReviewType
                .Include(a => a.EthicsEvaluation)
                    .ThenInclude(ev => ev.EthicsEvaluator)
                        .ThenInclude(f => f.Faculty)
                            .ThenInclude(u => u.User)
                .Include(a => a.EthicsEvaluation) // Include Chief details for each evaluation
                    .ThenInclude(ev => ev.Chief) // Include Chief navigation property in EthicsEvaluation
                    .ThenInclude(ap => ap.User)
                .ToListAsync();

            // Project data to the view model
            var viewModel = applications.Select(application => new ApplicationEvaluationsViewModel
            {
                UrecNo = application.urecNo,
                AppUser = application.User,
                InitialReview = application.InitialReview,
                EthicsEvaluations = application.EthicsEvaluation
            }).ToList();

            return View(viewModel);
        }



    }
}
