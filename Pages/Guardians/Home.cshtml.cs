using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians
{
    public class HomeModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;


        public ApplicationUser ApplicationUser { get; set; }

        public Guardian Guardian { get; set; }

        public List<Child> ChildrenList { get; set; }
        public IEnumerable<FamilyGroup> FamilyGroupList { get; set; }

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

            if (id == null)
            {
                return Redirect("/");
            }

            ApplicationUser = _unitOfWork.ApplicationUser.Get(a => a.Id == id);

            Guardian = _unitOfWork.Guardian.Get(g => g.GuardianID == ApplicationUser.GuardianId);

            FamilyGroupList = _unitOfWork.FamilyGroup.List(f => f.GuardianID == Guardian.GuardianID);

            ChildrenList = new List<Child>();

            if (FamilyGroupList.Any())
            {
                for (int i = 0; i < FamilyGroupList.Count(); i++)
                {
                    Child child = _unitOfWork.Child.Get(c => c.ChildID == FamilyGroupList.ElementAt(i).ChildID);
                    ChildrenList.Add(child);
                }
            }

            return Page();
        }
    }
}
