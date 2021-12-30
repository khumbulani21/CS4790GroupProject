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

namespace CS4790GroupProject.Pages.Guardians.Children
{
    public class UpsertModel : PageModel
    {
        // will represent a local instance of the db info
        private readonly IUnitOfWork _unitOfWork;

        // Bounded property in the cshtml that is passed through this cs and the html
        [BindProperty]
        public Child Child { get; set; }

        [BindProperty]
        public int DateYearPlaceHolder { get; set; }

        [BindProperty]
        public FamilyGroup familyGroup { get; set; }

        [BindProperty]
        public RelationshipType relationshipType { get; set; }

        [BindProperty]
        public string currentGuardianID { get; set; }


        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int ChildID, string GuardianID)
        {
            //checks if session is active and redirects to page to login again
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToPage("/SessionExpired");
            }
            currentGuardianID = GuardianID;

            if (ChildID != 0)
            {

                Child = _unitOfWork.Child.Get(i => i.ChildID == ChildID);
                if (Child == null)
                {
                    return NotFound();
                }

                ApplicationUser guardian = _unitOfWork.ApplicationUser.Get(g => g.Id == GuardianID);

                familyGroup = _unitOfWork.FamilyGroup.Get(f => f.ChildID == Child.ChildID &&
                    f.GuardianID == guardian.GuardianId);

                relationshipType = _unitOfWork.RelationshipType.Get(r => r.RelationshipID == familyGroup.RelationshipID);

              
            }
            else
            {
                Child = new Child();
                relationshipType = new RelationshipType();
                familyGroup = new FamilyGroup();
            }

            DateYearPlaceHolder = DateTime.Now.Year - 4;
            return Page(); 
        }

        public IActionResult OnPost()
        {
            DateTime currentTime = DateTime.Now;
            Child.ModifiedBy = currentGuardianID;
            Child.ModifiedDate = currentTime;

            relationshipType.ModifiedBy = currentGuardianID;
            relationshipType.ModifiedDate = currentTime;
            
            familyGroup.ModifiedBy = currentGuardianID;
            familyGroup.ModifiedDate = currentTime;

            // Get the guardian
            ApplicationUser guardian = _unitOfWork.ApplicationUser.Get(g => g.Id == currentGuardianID);


            if (Child.ChildID == 0)
            {
                // Child info is first
                _unitOfWork.Child.Add(Child);
                _unitOfWork.RelationshipType.Add(relationshipType);
               
                // Creating family group
                familyGroup.ChildID = Child.ChildID;
                familyGroup.GuardianID = Convert.ToInt32(guardian.GuardianId);
                familyGroup.RelationshipID = relationshipType.RelationshipID;
                _unitOfWork.FamilyGroup.Add(familyGroup);

                                                
            }
            else
            {
                _unitOfWork.Child.Update(Child);
                                
                _unitOfWork.RelationshipType.Update(relationshipType);
                _unitOfWork.FamilyGroup.Update(familyGroup);
                                             
            }
            _unitOfWork.Commit();
            return Redirect("/Guardians/Index/?id=" + currentGuardianID);
        }
    }
}
