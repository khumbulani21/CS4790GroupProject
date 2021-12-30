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

namespace CS4790GroupProject.Pages.Admin.Semesters
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public Semester Semester { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int SemesterID)
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

            if (SemesterID != 0)
            {
                Semester = _unitOfWork.Semester.Get(i => i.SemesterID == SemesterID);
                if (Semester == null)
                {
                    return NotFound();
                }
            }
            else
            {
                Semester = new Semester();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            //page needs validation to make sure these don't get set incorrectly
            if(Semester.StartDate >= Semester.EndDate)
            {
                return Page();
            }
            Semester.ModifiedBy = "Before Users";
            Semester.ModifiedDate = DateTime.Now;
            if (Semester.SemesterID == 0)
            {
                _unitOfWork.Semester.Add(Semester);
            }
            else
            {
                _unitOfWork.Semester.Update(Semester);
            }
            _unitOfWork.Commit();
            return RedirectToPage("/Admin/Semesters/Index");
        }
    }
}
