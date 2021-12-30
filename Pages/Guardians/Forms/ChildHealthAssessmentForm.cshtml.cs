using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians.Forms
{
    public class ChildHealthAssessmentFormModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ChildHealthAssessmentFormModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }



        [BindProperty]
        public HealthAssessment healthAssessment { get; set; }
        [BindProperty]
        public Application application { get; set; }


        public void OnGet(int id)
        {
            healthAssessment = new HealthAssessment();
            application = _unitOfWork.Application.Get(c => c.ChildID == id);
        }

        public IActionResult OnPost(int id)
        {
            healthAssessment.ApplicationID = application.ApplicationID;
            healthAssessment.ModifiedBy = "SYSTEM_USER";
            healthAssessment.ModifiedDate = DateTime.Now;

            if (healthAssessment.Medications == null)
            {
                healthAssessment.Medications = "No";
            }

            if (healthAssessment.Foods == null)
            {
                healthAssessment.Foods = "No";
            }

            if (healthAssessment.OtherAllergy == null)
            {
                healthAssessment.OtherAllergy = "No";
            }

            _unitOfWork.HealthAssessment.Add(healthAssessment);
            _unitOfWork.Commit();

            return RedirectToPage("/Guardians/Index");
        }
    }
}
