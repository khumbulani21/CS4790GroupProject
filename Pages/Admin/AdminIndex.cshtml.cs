using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.ViewModel;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CS4790GroupProject.Pages.Admin
{
    public class AdminIndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public List<AdminAbsenceViewModel> Absenses { get; set; }
        public List<ChildNote> Notes { get; set; }
        public List<AdminProgramViewModel> Programs { get; set; } = new List<AdminProgramViewModel>();
        public List<Application> Applications { get; set; }
        public AdminIndexModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public void OnGet()
        {
            LoadRecentAbsences();
            LoadRecentNotes();
            LoadRecentApplications();
            LoadPrograms();
        }

        private void LoadRecentApplications()
        {
            Applications = _unitOfWork.Application.List(i => i.ApplicationStatus.ToUpper() == "PENDING", null, "Child,Program").OrderByDescending(i => i.ModifiedDate).Take(20).ToList();
        }

        private void LoadPrograms()
        {
            var allPrograms = _unitOfWork.Program.List();
            foreach(var program in allPrograms)
            {
                Programs.Add(new AdminProgramViewModel
                {
                    Program = program,
                    PendingApplications = _unitOfWork.Application.List(i => i.ProgramID == program.ProgramID && i.ApplicationStatus.ToUpper() == "PENDING").Count(),
                    WaitlistedApplications = _unitOfWork.Application.List(i => i.ProgramID == program.ProgramID && i.ApplicationStatus.ToUpper() == "WAITLIST").Count()
                });
            }
        }

        public void LoadRecentAbsences()
        {
            //have to move this out since linq didn't like doing it in the query
            var sevenDaysAgo = DateTime.Today.AddDays(-7);
            var enrolledChildren = _unitOfWork.Enrollment.List(i => i.Semester.StartDate < DateTime.Today && i.Semester.EndDate > DateTime.Today, null, "Application.Child,Program").ToList();

            int daysAttended = 0;
            var checkAgainst = DateTime.Today;
            Absenses = new List<AdminAbsenceViewModel>();
            while (daysAttended < 7 && checkAgainst >= DateTime.Today.AddDays(-10))
            {
                var attendedThatDay = _unitOfWork.Attendance.List(i => i.AttendIn.Date == checkAgainst).ToList();
                if (attendedThatDay != null && attendedThatDay.Any())
                {
                    daysAttended++;
                    var absentChildren = enrolledChildren.Where(i => !attendedThatDay.Select(x => x.EnrollmentID).Contains(i.EnrollmentID)).ToList();
                    if (absentChildren != null && absentChildren.Any())
                    {
                        Absenses.Add(new AdminAbsenceViewModel { Date = checkAgainst.ToString("MM/dd/yyyy"), Children = absentChildren.ToList() });
                    }
                }
                checkAgainst = checkAgainst.AddDays(-1);
            }
        }

        public void LoadRecentNotes()
        {
            Notes = _unitOfWork.ChildNote.List(null, null, "Child").ToList();
        }
    }
}
