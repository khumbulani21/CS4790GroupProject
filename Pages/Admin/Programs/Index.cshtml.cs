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
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public IEnumerable<ApplicationCore.Models.Program> Programs { get; set; }

        public IndexModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public IActionResult OnGet()
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

            Programs = _unitOfWork.Program.List();

            return Page();
        }
    }
}
