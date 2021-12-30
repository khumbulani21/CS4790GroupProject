using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians
{
    public class ViewApplicationModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ViewApplicationModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public Application Application { get; set; }

        [BindProperty]
        public List<Application> application { get; set; }

        [BindProperty]
        public List<Child> child { get; set; }

        [BindProperty]
        public List<FamilyGroup> familygroup { get; set; }

        [BindProperty]
        public List<ApplicationCore.Models.Program> program { get; set; }

        [BindProperty]
        public ApplicationUser currentGuardian { get; set; }

        [BindProperty]
        public string currentGuardianID { get; set; }


        public IActionResult OnGet(string id)
        {
            //id = 9; //hard coding for now - this is the guardian id.

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

            currentGuardianID = id;
            // we get guardian aspnet user id because it is more encrypted
            // returns to homepage if there is no guardain logged in
            if (_unitOfWork.ApplicationUser.Get(a => a.Id == id) == null)
            {
                return Redirect("/");
            }

            currentGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == id);

            familygroup = _unitOfWork.FamilyGroup.List(c => c.GuardianID == currentGuardian.GuardianId).ToList();
            child = new List<Child>();
            application = _unitOfWork.Application.List().ToList();
            program = new List<ApplicationCore.Models.Program>();

            foreach (var item in familygroup)
            {
                Child c = _unitOfWork.Child.Get(x => x.ChildID == item.ChildID);
                child.Add(c);
            }

            foreach (var thing in application)
            {
                var p = _unitOfWork.Program.Get(p => p.ProgramID == thing.ProgramID);
                program.Add(p);
            }

            return Page();

        }


        public IActionResult OnPost(int id)
        {

            if (id != 0)
            {
                var applicationToDelete = _unitOfWork.Application.Get(c => c.ApplicationID == id);
                _unitOfWork.Application.Delete(applicationToDelete);
                _unitOfWork.Commit();
            }

            return Redirect("/Guardians/ViewApplication?id=" + currentGuardianID);
        }
    }
}
