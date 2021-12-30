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
    public class GuidanceBehaviorModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public GuidanceBehaviorModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public GuidanceAndBehavior guidanceBehavior { get; set; }

        [BindProperty]
        public Application application { get; set; }


        public void OnGet(int id)
        {
            guidanceBehavior = new GuidanceAndBehavior();
            application = _unitOfWork.Application.Get(c => c.ChildID == id);
        }

        public IActionResult OnPost(int id)
        {
            guidanceBehavior.ApplicationID = application.ApplicationID;
            guidanceBehavior.ModifiedBy = "SYSTEM_USER";
            guidanceBehavior.ModifiedDate = DateTime.Now;


            _unitOfWork.GuidanceAndBehavior.Add(guidanceBehavior);
            _unitOfWork.Commit();

            return RedirectToPage("/Guardians/Index");
        }
    }
}
