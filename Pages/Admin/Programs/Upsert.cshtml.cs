using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Admin.Programs
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ApplicationCore.Models.Program Program { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int ProgramID)
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

            if (ProgramID != 0)
            {
                Program = _unitOfWork.Program.Get(i => i.ProgramID == ProgramID);
                if (Program == null)
                {
                    return NotFound();
                }
            }
            else
            {
                Program = new ApplicationCore.Models.Program();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            Program.ModifiedBy = "Before Users";
            Program.ModifiedDate = DateTime.Now;
            if (Program.ProgramID == 0)
            {
                _unitOfWork.Program.Add(Program);
            }
            else
            {
                _unitOfWork.Program.Update(Program);
            }
            _unitOfWork.Commit();
            return RedirectToPage("/Admin/Programs/Index");
        }
    }
}
