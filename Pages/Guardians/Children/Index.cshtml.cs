using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians.Children
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public IEnumerable<Child> Children { get; set; }

        [BindProperty]
        public bool hasChild { get; set; }

        [BindProperty]
        public ApplicationUser currentGuardian { get; set; }

        public IEnumerable<FamilyGroup> familyGroupList { get; set; }

        public FamilyGroup familyGroup { get; set; }

        [BindProperty]
        public List<Child> childrenList { get; set; }

        public IndexModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;


        public IActionResult OnGet(string id)
        {
            //checks if session is active and redirects to page to login again
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToPage("/SessionExpired");
            }
            // we get guardian aspnet user id because it is more encrypted
            // returns to homepage if there is no guardain logged in
            if (_unitOfWork.ApplicationUser.Get(a => a.Id == id) == null)
            {
                return Redirect("/");
            }

            currentGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == id);

            // check if guardian has any children
            familyGroupList = _unitOfWork.FamilyGroup.List(f => f.GuardianID == currentGuardian.GuardianId);
            //_unitOfWork.Commit();
            
            if(familyGroupList.Count() == 0)
            {
                hasChild = false;
            }

            else
            {
                hasChild = true;
                childrenList = new List<Child>();

                for (int i = 0; i < familyGroupList.Count(); i++)
                {
                    if (familyGroupList.ElementAt(i) == null)
                    {
                        continue;
                    }
                    else
                    {
                        Child temp = _unitOfWork.Child.Get(c => c.ChildID == familyGroupList.ElementAt(i).ChildID);

                        childrenList.Add(temp);
                    }
                }
            }

            return Page();
        }
    }
}
