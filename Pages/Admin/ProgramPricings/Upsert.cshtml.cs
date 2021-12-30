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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CS4790GroupProject.Pages.Admin.ProgramPricings
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ProgramPricing ProgramPricing { get; set; }
        public List<SelectListItem> PriceTypes { get; set; }
        public List<SelectListItem> Programs { get; set; }
        public List<SelectListItem> Semesters { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int PricingID)
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

            PriceTypes = _unitOfWork.PriceType.List().Select(x =>
                           new SelectListItem { Value = x.PriceTypeID.ToString(), Text = x.PriceTypeDescription }).ToList();
            Semesters = _unitOfWork.Semester.List().Select(x =>
                           new SelectListItem { Value = x.SemesterID.ToString(), Text = x.SemesterName }).ToList();
            Programs = _unitOfWork.Program.List().Select(x =>
                            new SelectListItem { Value = x.ProgramID.ToString(), Text = x.ProgramName }).ToList();
            if (PricingID != 0)
            {
                ProgramPricing = _unitOfWork.ProgramPricing.Get(i => i.PricingID == PricingID);
                if (ProgramPricing == null)
                {
                    return NotFound();
                }
            }
            else
            {
                ProgramPricing = new ProgramPricing();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            ProgramPricing.ModifiedBy = "Before Users";
            ProgramPricing.ModifiedDate = DateTime.Now;
            if (ProgramPricing.PricingID == 0)
            {
                _unitOfWork.ProgramPricing.Add(ProgramPricing);
            }
            else
            {
                _unitOfWork.ProgramPricing.Update(ProgramPricing);
            }
            _unitOfWork.Commit();
            return RedirectToPage("/Admin/ProgramPricings/Index");
        }
    }
}
