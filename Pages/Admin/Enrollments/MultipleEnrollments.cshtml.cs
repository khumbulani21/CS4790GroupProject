using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CS4790GroupProject.Pages.Admin.Enrollments
{
    public class MultipleEnrollmentsModel : PageModel
    {

        public IUnitOfWork _unitOfWork { get; set; }
        public MultipleEnrollmentsModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        [BindProperty]
        public MultipleEnrollmentsVM EnrollmentObj { get; set; }
        [BindProperty]
        public MultipleEnrollmentsVM ToEnrollmentObj { get; set; }
        public void OnGet()
        {
            Enrollment enrollment = new Enrollment();
            Application application = new Application();


            var semesters = _unitOfWork.Semester.List(s => s.StartDate > DateTime.Now || (s.StartDate < DateTime.Now && s.EndDate > DateTime.Now) || s.SemesterID == enrollment.SemesterID);
            var programs = _unitOfWork.Program.List();
            //var applications = _unitOfWork.Application.List(a => a.ApplicationID == a.ApplicationID, null, "Child,Program"); ;

            EnrollmentObj = new MultipleEnrollmentsVM
            {
                SemesterID = 0,
                ProgramID = 0,
                SemesterList = semesters.Select(c => new SelectListItem { Value = c.SemesterID.ToString(), Text = c.SemesterName }),
                ProgramList = programs.Select(p => new SelectListItem { Value = p.ProgramID.ToString(), Text = p.ProgramName })

            };
            ToEnrollmentObj = new MultipleEnrollmentsVM
            {
                SemesterID = 0,
                ProgramID = 0,

                SemesterList = semesters.Select(c => new SelectListItem { Value = c.SemesterID.ToString(), Text = c.SemesterName }),
                ProgramList = programs.Select(p => new SelectListItem { Value = p.ProgramID.ToString(), Text = p.ProgramName })

            };
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {

                //get all enrollments and enroll in new semester
                IEnumerable<Enrollment> enrollments = _unitOfWork.Enrollment.List(e => e.ProgramID == EnrollmentObj.ProgramID && e.SemesterID == EnrollmentObj.SemesterID);
                List<Enrollment> enrollmentList = enrollments.ToList();
                foreach (var item in enrollmentList)
                {
                    //create a new enrollment
                    Enrollment enrollment = new Enrollment();
                    enrollment.ApplicationID = item.ApplicationID;
                    enrollment.HourBlock = item.HourBlock;
                    enrollment.ProgramID = ToEnrollmentObj.ProgramID;
                    enrollment.SemesterID = ToEnrollmentObj.SemesterID;
                    enrollment.ModifiedDate = DateTime.Now;
                    enrollment.ModifiedBy = claim.Value;
                    _unitOfWork.Enrollment.Add(enrollment);
                    _unitOfWork.Commit();
                }

            }
            return RedirectToPage("/Admin/Enrollments/Index");

        }

    }
}
