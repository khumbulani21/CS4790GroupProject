using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.ViewModel;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CS4790GroupProject.Pages.Admin.Applications
{
    public class ApplicationInfoModel : PageModel
    {

        public IUnitOfWork _unitOfWork { get; set; }
       
        public ApplicationInfoModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
      
        public Application Application { get; set; }
        [BindProperty]
        public ApplicationInfoVM ApplicationInfoObj { get; set; }
      
        public void OnGet(int? id)
        {
           
            Application = _unitOfWork.Application.Get(a => a.ApplicationID == id, false, "Child,Program");
            var semesters = _unitOfWork.Semester.List(s => s.StartDate > DateTime.Now || (s.StartDate < DateTime.Now && s.EndDate > DateTime.Now) );
            var programs = _unitOfWork.Program.List();
            int zero = 0;
            ApplicationInfoObj = new ApplicationInfoVM
            {
                Enrollment = new Enrollment(),
                Program = Application.Program,
                Child = Application.Child,
                Age = DateTime.Now.Subtract(Application.Child.ChildDOB).Days / 365,
                Enrollments = _unitOfWork.Enrollment.List(e => e.ApplicationID == Application.ApplicationID, null, "Semester,Program").ToList(),
                ProgramList = programs.Select(c => new SelectListItem { Value = c.ProgramID.ToString(), Text = c.ProgramName })
                .Append(new SelectListItem { Value = zero.ToString(), Text = " Please select a program" }).OrderBy(c => c.Text),
                HourBlocks = SD.HourBlocks.Select(p => new SelectListItem { Value = p.Value.ToString(), Text = p.Text })
                .Append(new SelectListItem { Value = zero.ToString(), Text = " Please select a hour block" }).OrderBy(c => c.Text),
                SemesterList =semesters.Select(c => new SelectListItem { Value = c.SemesterID.ToString(), Text = c.SemesterName })
                .Append(new SelectListItem { Value = zero.ToString(), Text = " Please select a semester" }).OrderBy(c=>c.Text)
            };
          }

        public IActionResult OnPostApprove(int id)
        {
          
            var claimsIdentity = (ClaimsIdentity)User.Identity;
             
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string modifiedBy = null;
            if (claim!=null)
            {
                modifiedBy = claimsIdentity.Name;
                if (!ModelState.IsValid)
                {
                    return Page();
                }
               Application app = _unitOfWork.Application.Get(c => c.ApplicationID == id);
               var duplicateEnrollment = _unitOfWork.Enrollment.Get(e => e.ApplicationID == id && e.SemesterID== ApplicationInfoObj.Enrollment.SemesterID);
               //check for duplicate enrollment
                if (duplicateEnrollment != null)
                {
                    return RedirectToPage("/Admin/Applications/ApplicationInfo", new { id = id });
                }

                Enrollment enrollment = new Enrollment();
                enrollment.ApplicationID = id;
                enrollment.HourBlock = ApplicationInfoObj.Enrollment.HourBlock;
                enrollment.ProgramID = ApplicationInfoObj.Enrollment.ProgramID;
                enrollment.SemesterID = ApplicationInfoObj.Enrollment.SemesterID;
                enrollment.ModifiedBy = modifiedBy;
                enrollment.ModifiedDate = DateTime.Now;
                if (app != null)
                {
                    //only change status if it's not set to approved
                    if (app.ApplicationStatus!="Approved")
                    {
                        app.ApplicationStatus = "Approved";
                        app.ModifiedBy = modifiedBy;
                        app.ModifiedDate = DateTime.Now;
                    }
                    
                    _unitOfWork.Application.Update(app);
                    _unitOfWork.Enrollment.Add(enrollment);
                    _unitOfWork.Commit();
                }
            return RedirectToPage("/Admin/Applications/ApplicationInfo", new { id = id });
            }
            return Page();
        }
        public IActionResult OnPostDeny(int id)
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            Application app = _unitOfWork.Application.Get(c => c.ApplicationID == id);
            if (app != null)
            {
                app.ApplicationStatus = "Denied";
                _unitOfWork.Application.Update(app);
                _unitOfWork.Commit();
            }




            return RedirectToPage("/Admin/Applications/Index");
        }
        public IActionResult OnPostPending(int id)
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            Application app = _unitOfWork.Application.Get(c => c.ApplicationID == id);
            if (app != null)
            {
                app.ApplicationStatus = "Pending";
                _unitOfWork.Application.Update(app);
                _unitOfWork.Commit();
            }
            return RedirectToPage("/Admin/Applications/Index");
        }
        public IActionResult OnPostWaitlist(int id)
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            Application app = _unitOfWork.Application.Get(c => c.ApplicationID == id);
            if (app != null)
            {
                app.ApplicationStatus = "Waitlist";
                _unitOfWork.Application.Update(app);
                _unitOfWork.Commit();
            }
            return RedirectToPage("/Admin/Applications/Index");
        }
    }
}

