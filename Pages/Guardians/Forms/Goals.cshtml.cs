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
    public class GoalsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public GoalsModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public ChildGoals childGoals { get; set; }

        [BindProperty]
        public Application application { get; set; }


        public void OnGet(int id)
        {
            childGoals = new ChildGoals();
            application = _unitOfWork.Application.Get(c => c.ChildID == id);
        }

        public IActionResult OnPost(int id)
        {
            childGoals.ApplicationID = application.ApplicationID;
            childGoals.ModifiedBy = "SYSTEM_USER";
            childGoals.ModifiedDate = DateTime.Now;


            _unitOfWork.ChildGoals.Add(childGoals);
            _unitOfWork.Commit();

            return RedirectToPage("/Guardians/Index");
        }
    }
}
