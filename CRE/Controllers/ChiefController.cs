﻿using CRE.Interfaces;
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
            var approvedApplications = await _initialReviewServices.GetApprovedApplicationsAsync();

            var viewModel = new ApprovedApplicationListViewModel
            {
                ApprovedApplications = approvedApplications.Select(app => new ApprovedApplicationViewModel
                {
                    AppUser = app.AppUser,
                    Secretariat = app.Secretariat,
                    NonFundedResearchInfo = app.NonFundedResearchInfo,
                    CoProponent = app.CoProponent,
                    ReceiptInfo = app.ReceiptInfo,
                    Chairperson = app.Chairperson,
                    EthicsEvaluator = app.EthicsEvaluator,
                    EthicsApplication = app.EthicsApplication,
                    InitialReview = app.InitialReview,
                    EthicsApplicationForms = app.EthicsApplicationForms,
                    EthicsApplicationLog = app.EthicsApplicationLog
                }).ToList()
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Details(string urecNo)
        {
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo); // Assume this method fetches the application details

            if (applicationDetails == null)
            {
                return NotFound();
            }

            // Assuming you want to map this to AssignReviewTypeViewModel
            var viewModel = new AssignReviewTypeViewModel
            {
                AppUser = applicationDetails.AppUser,
                Secretariat = applicationDetails.Secretariat,
                NonFundedResearchInfo = applicationDetails.NonFundedResearchInfo,
                CoProponent = applicationDetails.CoProponent,
                ReceiptInfo = applicationDetails.ReceiptInfo,
                Chairperson = applicationDetails.Chairperson,
                EthicsEvaluator = applicationDetails.EthicsEvaluator,
                EthicsApplication = applicationDetails.EthicsApplication,
                InitialReview = applicationDetails.InitialReview,
                EthicsApplicationForms = applicationDetails.EthicsApplicationForms,
                EthicsApplicationLog = applicationDetails.EthicsApplicationLog,
               
            };

            return View(viewModel);
        }


    }
}
