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

namespace CS4790GroupProject.Pages.Admin.PriceTypes
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public PriceType PriceType { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int PriceTypeID)
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

            if (PriceTypeID != 0)
            {
                PriceType = _unitOfWork.PriceType.Get(i => i.PriceTypeID == PriceTypeID);
                if (PriceType == null)
                {
                    return NotFound();
                }
            }
            else
            {
                PriceType = new PriceType();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            PriceType.ModifiedBy = "Before Users";
            PriceType.ModifiedDate = DateTime.Now;
            if (PriceType.PriceTypeID == 0)
            {
                _unitOfWork.PriceType.Add(PriceType);
            }
            else
            {
                _unitOfWork.PriceType.Update(PriceType);
            }
            _unitOfWork.Commit();
            return RedirectToPage("/Admin/PriceTypes/Index");
        }
    }
}
