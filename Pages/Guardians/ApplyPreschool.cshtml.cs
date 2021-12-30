using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.ViewModel;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CS4790GroupProject.Pages.Guardians
{
    public class ApplyPreschoolModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ApplyPreschoolModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public List<ApplicationCore.Models.Program> program { get; set; }

        [BindProperty]
        public List<FamilyGroup> familygroup { get; set; }

        [BindProperty]
        public Application application { get; set; }

        [BindProperty]
        public List<Child> child { get; set; }

        [BindProperty]
        public GuardianApply applyObj { get; set; }

        [BindProperty]
        public ApplicationUser currentGuardian { get; set; }

        [BindProperty]
        public string currentGuardianID { get; set; }

        public IActionResult OnGet(string id)
        {
            //checks if session is active and redirects to page to login again
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToPage("/SessionExpired");
            }

            //ensures that only the user in the guardian role can access this page
            if (!User.IsInRole(SD.GuardianRole))
            {
                return Forbid();
            }

            //id = 9; //Hard coding Guardian ID for now. Will change later
            currentGuardianID = id;
            program = _unitOfWork.Program.List().ToList();

            // we get guardian aspnet user id because it is more encrypted
            // returns to homepage if there is no guardain logged in
            if (_unitOfWork.ApplicationUser.Get(a => a.Id == id) == null)
            {
                return Redirect("/");
            }

            currentGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == id);

            familygroup = _unitOfWork.FamilyGroup.List(c => c.GuardianID == currentGuardian.GuardianId).ToList();
            child = new List<Child>();

            foreach (var item in familygroup)
            {
                Child c = _unitOfWork.Child.Get(x => x.ChildID == item.ChildID);
                child.Add(c);
            }

            applyObj = new GuardianApply
            {
                application = new Application(),
                ChildList = child.Select(c => new SelectListItem { Value = c.ChildID.ToString(), Text = c.FullName }),

            };

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            application.ApplicationStatus = "Pending";
            application.ProgramID = id;
            application.ChildID = applyObj.application.ChildID;
            application.RequestedHours = applyObj.application.RequestedHours;
            application.ModifiedDate = DateTime.Now;
            application.ModifiedBy = "Guardian Submission";

            _unitOfWork.Application.Add(application);
            _unitOfWork.Commit();

            return Redirect("/Guardians/Index?id=" + currentGuardianID);


        }
    }
}
