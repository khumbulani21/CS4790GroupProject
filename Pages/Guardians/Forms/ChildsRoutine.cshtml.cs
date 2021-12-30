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
    public class ChildsRoutineModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ChildsRoutineModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public ChildsRoutine childsRoutine { get; set; }

        [BindProperty]
        public Application application { get; set; }


        public void OnGet(int id)
        {
            childsRoutine = new ChildsRoutine();
            application = _unitOfWork.Application.Get(c => c.ChildID == id);
        }

        public IActionResult OnPost(int id)
        {
            childsRoutine.ApplicationID = application.ApplicationID;
            childsRoutine.ModifiedBy = "SYSTEM_USER";
            childsRoutine.ModifiedDate = DateTime.Now;

            if (childsRoutine.routineForGoodbyes == null)
            {
                childsRoutine.routineForGoodbyes = "No";
            }

            _unitOfWork.ChildsRoutine.Add(childsRoutine);
            _unitOfWork.Commit();

            return RedirectToPage("/Guardians/Index");
        }
    }
}
