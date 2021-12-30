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
    public class UpsertModel : PageModel
    {
        public IUnitOfWork _unitOfWork { get; set; }
        [BindProperty]
        public EnrollmentVM EnrollmentObj { get; set; }
        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public IActionResult OnGet(int? applicationID,int? enrollmentID)
        {
            Enrollment enrollment = new Enrollment();
            Application application = new Application();
            Semester semester = new Semester();
            ApplicationCore.Models.Child child = new ApplicationCore.Models.Child();
            if (enrollmentID != 0 && applicationID!=0)
            {
                //enrollment id submitted so find the application
                //you update
                  enrollment = _unitOfWork.Enrollment.Get(e => e.EnrollmentID == enrollmentID);
                  application = _unitOfWork.Application.Get(a => a.ApplicationID == applicationID);
                 child = _unitOfWork.Child.Get(c => c.ChildID == application.ChildID);
                semester = _unitOfWork.Semester.Get(s => s.SemesterID == enrollment.SemesterID);
            }
            else if (enrollmentID == 0 && applicationID != 0)
            {
                //create new enrollment from enrollment page
                //need list of applications
                application = _unitOfWork.Application.Get(a => a.ApplicationID == applicationID);
                child = _unitOfWork.Child.Get(c => c.ChildID == application.ChildID);
                //semester = _unitOfWork.Semester.Get(s => s.StartDate>= DateTime.Now && DateTime.Now< s.EndDate);
            }
            else
            {
                
                //need to be selected
                //semester = _unitOfWork.Semester.Get(s => s.StartDate >= DateTime.Now && DateTime.Now < s.EndDate);
            }
  
                var semesters = _unitOfWork.Semester.List(s => s.StartDate > DateTime.Now || (s.StartDate < DateTime.Now && s.EndDate > DateTime.Now) || s.SemesterID == enrollment.SemesterID);
                var programs = _unitOfWork.Program.List();
                var applications = _unitOfWork.Application.List( a=>a.ApplicationID== a.ApplicationID, null, "Child,Program"); ;
                int zero = 0;
                EnrollmentObj = new EnrollmentVM
                {
                    Enrollment = enrollment,
                    Application=application,
                    Enrollments = _unitOfWork.Enrollment.List(e => e.ApplicationID == application.ApplicationID, null, "Semester,Program").ToList().OrderBy(e=>e.Program.ProgramName),
                    Semester =semester,
                    Child = child,
                    ApplicationList= applications.Select(a => new SelectListItem { Value = a.ApplicationID.ToString(), Text = a.Child.ChildFirst+" "+a.Child.ChildLast }).Append(new SelectListItem { Value = zero.ToString(), Text = " -Please select an application by child name -" }).OrderBy(c => c.Text),
                    SemesterList = semesters.Select(c => new SelectListItem { Value = c.SemesterID.ToString(), Text = c.SemesterName }).Append(new SelectListItem { Value = zero.ToString(), Text = " -Please select a semester -" }).OrderBy(c => c.Text),
                    ProgramList = programs.Select(p => new SelectListItem { Value = p.ProgramID.ToString(), Text = p.ProgramName }).Append(new SelectListItem { Value = zero.ToString(), Text = " -Please select a program -" }).OrderBy(c => c.Text),
                    HourBlocks = SD.HourBlocks.Select(p => new SelectListItem { Value = p.Value.ToString(), Text = p.Text }).Append(new SelectListItem { Value = zero.ToString(), Text = " -Please select hours -" }).OrderBy(c => c.Text)
                };
      
            

            return Page();
        }

        public IActionResult OnPost(int enrollmentID)
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                Enrollment enrollment = _unitOfWork.Enrollment.Get(e => e.EnrollmentID == enrollmentID);
                if (enrollment.SemesterID!=EnrollmentObj.Enrollment.SemesterID)
                {
                    var enrollments = _unitOfWork.Enrollment.Get(e=>e.EnrollmentID==enrollment.EnrollmentID&&e.SemesterID==EnrollmentObj.Enrollment.SemesterID);
                    if (enrollments!=null)
                    {
                        //duplicate don't save
                    }
                }
               
                enrollment.HourBlock = EnrollmentObj.Enrollment.HourBlock;
                enrollment.ProgramID = EnrollmentObj.Enrollment.ProgramID;
                enrollment.SemesterID = EnrollmentObj.Enrollment.SemesterID;
                enrollment.ModifiedBy = claim.Value;
                enrollment.ModifiedDate = DateTime.Now;
                _unitOfWork.Enrollment.Update(enrollment);
                _unitOfWork.Commit();
                return RedirectToPage("/Admin/Enrollments/Upsert", new { applicationID=enrollment.ApplicationID,enrollmentID = enrollmentID });
            }
            else
            {
                return Page();
            }
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
                    newEnrollment.ApplicationID = applicationID;
                    newEnrollment.HourBlock = EnrollmentObj.Enrollment.HourBlock;
                    newEnrollment.ProgramID = EnrollmentObj.Enrollment.ProgramID;
                    newEnrollment.SemesterID = EnrollmentObj.Enrollment.SemesterID;
                    newEnrollment.ModifiedBy = claim.Value;
                    newEnrollment.ModifiedDate = DateTime.Now;
                    _unitOfWork.Enrollment.Add(newEnrollment);
                    _unitOfWork.Commit();
                return RedirectToPage("/Admin/Enrollments/Upsert", new { applicationID = applicationID, enrollmentID = newEnrollment.EnrollmentID });
            }
            else
            {
                return Page();
            }
            
        }
    }
}
