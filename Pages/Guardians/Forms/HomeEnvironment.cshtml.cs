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
    public class HomeEnvironmentModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeEnvironmentModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public HomeEnvironmentFamily homeEnvironment { get; set; }

        [BindProperty]
        public Application application { get; set; }


        public void OnGet(int id)
        {
            homeEnvironment = new HomeEnvironmentFamily();
            application = _unitOfWork.Application.Get(c => c.ChildID == id);
        }

        public IActionResult OnPost(int id)
        {
            homeEnvironment.ApplicationID = application.ApplicationID;
            homeEnvironment.ModifiedBy = "SYSTEM_USER";
            homeEnvironment.ModifiedDate = DateTime.Now;

            if (homeEnvironment.routinelyCaredForByOthers == null)
            {
                homeEnvironment.routinelyCaredForByOthers = "No";
            }

            if (homeEnvironment.prolongedAbsencePrimaryCaregiver == null)
            {
                homeEnvironment.prolongedAbsencePrimaryCaregiver = "No";
            }

            if (homeEnvironment.upcomingProlongedAbsencePrimaryCaregiver == null)
            {
                homeEnvironment.upcomingProlongedAbsencePrimaryCaregiver = "No";
            }

            if (homeEnvironment.pets == null)
            {
                homeEnvironment.pets = "No";
            }


            _unitOfWork.HomeEnvironmentFamily.Add(homeEnvironment);
            _unitOfWork.Commit();

            return RedirectToPage("/Guardians/Index");
        }
    }
}
