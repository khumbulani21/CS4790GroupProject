using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Admin.Child
{
    public class ViewModel : PageModel
    {
        IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ApplicationCore.Models.Child Child { get; set; }
        public List<Attendance> Attendances { get; set; }
        public List<string> Absences { get; set; }
        public HealthAssessment healthAssessment { get; set; } = null;
        public ChildsRoutine childsRoutine { get; set; } = null;
        public ChildGoals childGoals { get; set; } = null;
        public GuidanceAndBehavior guidanceBehavior { get; set; } = null;
        public HomeEnvironmentFamily homeEnvironment { get; set; } = null;
        public List<ContactInfo> ContactInfos { get; set; }
        public EmergencyCardForm MedicalContacts { get; set; }
        public ViewModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment) { _unitOfWork = unitOfWork; _hostingEnvironment = hostingEnvironment; }
        [BindProperty]
        public ChildNote NoteForm { get; set; }
        public IActionResult OnGet(int ChildID)
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

            LoadChild(ChildID);
            LoadAttendance(ChildID);
            LoadContactInfo(ChildID);
            LoadFormInfo(ChildID);
            return Page();
        }

        private void LoadContactInfo(int childID)
        {
            ContactInfos = _unitOfWork.ContactInfo.List(i => i.ChildID == childID, null, "ContactType,AuthorizedToPickUp").ToList();
            MedicalContacts = _unitOfWork.EmergencyCardForm.Get(i => i.ChildID == childID);
        }

        private void LoadFormInfo(int childID)
        {
            if (Child.Applications != null && Child.Applications.Any())
            {
                healthAssessment = _unitOfWork.HealthAssessment.Get(i => i.ApplicationID == Child.Applications[0].ApplicationID);
                childsRoutine = _unitOfWork.ChildsRoutine.Get(i => i.ApplicationID == Child.Applications[0].ApplicationID);
                childGoals = _unitOfWork.ChildGoals.Get(i => i.ApplicationID == Child.Applications[0].ApplicationID);
                guidanceBehavior = _unitOfWork.GuidanceAndBehavior.Get(i => i.ApplicationID == Child.Applications[0].ApplicationID);
                homeEnvironment = _unitOfWork.HomeEnvironmentFamily.Get(i => i.ApplicationID == Child.Applications[0].ApplicationID);
            }
        }

        public IActionResult OnPost()
        {
            string imageLocation = "";
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            if (files.Any())
            {
                string fileName = Guid.NewGuid().ToString();
                var uploadPath = Path.Combine(webRootPath, @"images\NoteImages\");
                var extension = Path.GetExtension(files.First().FileName);
                var fullPath = uploadPath + fileName + extension;

                using (var fileStream = System.IO.File.Create(fullPath))
                {
                    files[0].CopyTo(fileStream);
                }

                imageLocation = @"\images\NoteImages\" + fileName + extension;
            }
            if (NoteForm.ChildNoteID == 0)
            {
                NoteForm.EnteredOn = DateTime.Now;
                if (!string.IsNullOrEmpty(imageLocation)) 
                {
                    NoteForm.Image = imageLocation;
                }
                _unitOfWork.ChildNote.Add(NoteForm);
            }
            else
            {
                var previousEntry = _unitOfWork.ChildNote.Get(i => i.ChildNoteID == NoteForm.ChildNoteID);
                previousEntry.Note = NoteForm.Note;
                previousEntry.IsPublic = NoteForm.IsPublic;
                if (!string.IsNullOrEmpty(imageLocation))
                {
                    NoteForm.Image = imageLocation;
                }
                _unitOfWork.ChildNote.Update(previousEntry);
            }
            _unitOfWork.Commit();
            LoadChild(NoteForm.ChildID);
            LoadAttendance(NoteForm.ChildID);

            return Page();
        }

        public void LoadChild(int ChildID)
        {
            Child = _unitOfWork.Child.Get(i => i.ChildID == ChildID, true, "FamilyGroups.Guardian.ContactInfos,FamilyGroups.RelationshipType,Applications.Program,EmergencyContacts.ContactInfo.ContactType,Notes");
        }

        public void LoadAttendance(int ChildID)
        {

            var latestEnrollent = _unitOfWork.Enrollment.List(i => i.Application.ChildID == ChildID, null, "Application,Semester").OrderBy(i => i.Semester.StartDate).FirstOrDefault();
            if(latestEnrollent != null)
            {
                var distinctDays = _unitOfWork.Attendance.List(i => i.Enrollment.ProgramID == latestEnrollent.ProgramID).OrderByDescending(i => i.AttendIn).Select(x => x.AttendIn.ToString("MM/dd/yyyy")).Distinct().ToList();
                Attendances = _unitOfWork.Attendance.List(i => i.EnrollmentID == latestEnrollent.EnrollmentID).ToList();
                Absences = distinctDays.Where(i => !Attendances.Select(x => x.AttendIn.ToString("MM/dd/yyyy")).Contains(i)).ToList();
            }
        }
    }
}
