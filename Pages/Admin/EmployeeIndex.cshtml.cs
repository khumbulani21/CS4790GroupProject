using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Admin
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
            if (User.IsInRole(SD.GuardianRole))
            {
                return Forbid();
            }

            Programs = _unitOfWork.Program.List(null, null, "Enrollments.Application.Child,Enrollments.Attendances,Enrollments.Semester").ToList();
            //only get attendance for today
            foreach(var program in Programs)
            {
                program.Enrollments = program.Enrollments.Where(i => i.Semester.StartDate <= DateTime.Today && i.Semester.EndDate >= DateTime.Today).ToList();
                foreach (var enrollment in program.Enrollments)
                {
                    enrollment.Attendances = enrollment.Attendances.Where(
                        i => i.AttendIn.ToShortDateString() == DateTime.Now.ToShortDateString()).ToList();
                }
            }
            return Page();
        }
    }
}
