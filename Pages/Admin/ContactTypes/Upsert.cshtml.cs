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

namespace CS4790GroupProject.Pages.Admin.ContactTypes
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ContactType ContactType { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int ContactTypeID)
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

            if (ContactTypeID != 0)
            {
                ContactType = _unitOfWork.ContactType.Get(i => i.ContactTypeID == ContactTypeID);
                if (ContactType == null)
                {
                    return NotFound();
                }
            }
            else
            {
                ContactType = new ContactType();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            ContactType.ModifiedBy = "Before Users";
            ContactType.ModifiedDate = DateTime.Now;
            if (ContactType.ContactTypeID == 0)
            {
                _unitOfWork.ContactType.Add(ContactType);
            }
            else
            {
                _unitOfWork.ContactType.Update(ContactType);
            }
            _unitOfWork.Commit();
            return RedirectToPage("/Admin/ContactTypes/Index");
        }
    }
}
