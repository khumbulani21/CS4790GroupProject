using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.ViewModel;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CS4790GroupProject.Pages.Admin.Enrollments
{
    public class NewEnrollmentModel : PageModel
    {

        public IUnitOfWork _unitOfWork { get; set; }
        [BindProperty]
        public EnrollmentVM EnrollmentObj { get; set; }
        public NewEnrollmentModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public IActionResult OnGet()
        {
            Enrollment enrollment = new Enrollment();
            Application application = new Application();
            Semester semester = new Semester();
            ApplicationCore.Models.Child child = new ApplicationCore.Models.Child();
           
          
     

            var semesters = _unitOfWork.Semester.List(s => s.StartDate > DateTime.Now || (s.StartDate < DateTime.Now && s.EndDate > DateTime.Now) );
            var programs = _unitOfWork.Program.List();
            var applications = _unitOfWork.Application.List(a => a.ApplicationID == a.ApplicationID, null, "Child,Program"); ;
            int zero = 0;

            EnrollmentObj = new EnrollmentVM
            {
                Enrollment = enrollment,
                Application = application,
                Enrollments = _unitOfWork.Enrollment.List(e => e.ApplicationID == application.ApplicationID, null, "Semester,Program").ToList().OrderBy(e => e.Program.ProgramName),
                Semester = semester,
      
                ApplicationList = applications.Select(a => new SelectListItem { Value = a.ApplicationID.ToString(), Text = a.Child.ChildFirst + " " + a.Child.ChildLast }).Append(new SelectListItem { Value = zero.ToString(), Text = " -Please select an application by child name -" }).OrderBy(c => c.Text),
                SemesterList = semesters.Select(c => new SelectListItem { Value = c.SemesterID.ToString(), Text = c.SemesterName }).Append(new SelectListItem { Value = zero.ToString(), Text = " -Please select a semester -" }).OrderBy(c => c.Text),
                ProgramList = programs.Select(p => new SelectListItem { Value = p.ProgramID.ToString(), Text = p.ProgramName }).Append(new SelectListItem { Value = zero.ToString(), Text = " -Please select a program -" }).OrderBy(c => c.Text),
                HourBlocks = SD.HourBlocks.Select(p => new SelectListItem { Value = p.Value.ToString(), Text = p.Text }).Append(new SelectListItem { Value = zero.ToString(), Text = " -Please select hours -" }).OrderBy(c => c.Text)
            };



            return Page();
        }

        public IActionResult OnPostCreate(int applicationID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {

                Enrollment newEnrollment = new Enrollment();
                newEnrollment.ApplicationID = EnrollmentObj.Enrollment.ApplicationID;
                newEnrollment.HourBlock = EnrollmentObj.Enrollment.HourBlock;
                newEnrollment.ProgramID = EnrollmentObj.Enrollment.ProgramID;
                newEnrollment.SemesterID = EnrollmentObj.Enrollment.SemesterID;
                newEnrollment.ModifiedBy = claim.Value;
                newEnrollment.ModifiedDate = DateTime.Now;
                _unitOfWork.Enrollment.Add(newEnrollment);
                _unitOfWork.Commit();
                return RedirectToPage("/Admin/Enrollments/Upsert", new { applicationID = newEnrollment.ApplicationID, enrollmentID = newEnrollment.EnrollmentID });
            }
            else
            {
                return Page();
            }

        }
    }
}
