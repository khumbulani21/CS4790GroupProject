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

namespace CS4790GroupProject.Pages.Admin.Volunteers.Opportunities
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public VolunteerOpportunity Opportunity { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int OpportunityID)
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

            if (OpportunityID != 0)
            {
                Opportunity = _unitOfWork.VolunteerOpportunity.Get(i => i.VolunteerOpID == OpportunityID);
                if (Opportunity == null)
                {
                    return NotFound();
                }
            }
            else
            {
                Opportunity = new VolunteerOpportunity();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            Opportunity.ModifiedBy = "Before Users";
            Opportunity.ModifiedDate = DateTime.Now;
            if (Opportunity.VolunteerOpID == 0)
            {
                _unitOfWork.VolunteerOpportunity.Add(Opportunity);
            }
            else
            {
                _unitOfWork.VolunteerOpportunity.Update(Opportunity);
            }
            _unitOfWork.Commit();
            return RedirectToPage("/Admin/Volunteers/Opportunities/Index");
        }
    }
}
