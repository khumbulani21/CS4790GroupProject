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

namespace CS4790GroupProject.Pages.Admin.Guardians
{
    public class ViewModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ViewModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public Guardian Guardian { get; set; }
        public IActionResult OnGet(int GuardianID)
        {
            //checks if session is active and redirects to page to login again
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToPage("/SessionExpired");
            }

            //ensures that only the user in the guardian role can access this page
            if (User.IsInRole(SD.GuardianRole))
            {
                return Forbid();
            }

            Guardian = _unitOfWork.Guardian.Get(i => i.GuardianID == GuardianID, false, "FamilyGroups.Child,FamilyGroups.RelationshipType,Address,VolunteeredFor.VolunteerOpportunity,ContactInfos.ContactType,ContactInfos.Child");

            return Page();
        }

    }
}
