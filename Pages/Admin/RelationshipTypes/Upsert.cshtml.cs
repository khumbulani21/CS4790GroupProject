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

namespace CS4790GroupProject.Pages.Admin.RelationshipTypes
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public RelationshipType RelationshipType { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int RelationshipID)
        {
            //checks if session is active and redirects to page to login again
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToPage("/SessionExpired");
            }

            //ensures that only the user in the guardian role can access this page
            if (!User.IsInRole(SD.AdminRole))
            {
                return Forbid();
            }

            if (RelationshipID != 0)
            {
                RelationshipType = _unitOfWork.RelationshipType.Get(i => i.RelationshipID == RelationshipID);
                if (RelationshipType == null)
                {
                    return NotFound();
                }
            }
            else
            {
                RelationshipType = new RelationshipType();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            RelationshipType.ModifiedBy = "Before Users";
            RelationshipType.ModifiedDate = DateTime.Now;
            if (RelationshipType.RelationshipID == 0)
            {
                _unitOfWork.RelationshipType.Add(RelationshipType);
            }
            else
            {
                _unitOfWork.RelationshipType.Update(RelationshipType);
            }
            _unitOfWork.Commit();
            return RedirectToPage("/Admin/RelationshipTypes/Index");
        }
    }
}
