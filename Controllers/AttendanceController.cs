using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpPost("ProcessCheckIn/{enrollmentID}")]
        public IActionResult ProcessCheckIn(int enrollmentID)
        {
            var enrollment = _unitOfWork.Enrollment.Get(i => i.EnrollmentID == enrollmentID);
            if (enrollment == null)
            {
                return NotFound();
            }
            var attendance = new Attendance
            {
                EnrollmentID = enrollmentID,
                AttendIn = DateTime.Now
            };
            _unitOfWork.Attendance.Add(attendance);
            _unitOfWork.Commit();
            return Json( new { AttendanceID = attendance.AttendanceID, CheckedIn = attendance.AttendIn.ToString("hh:mm tt") });
        }

        [HttpPost("ProcessCheckOut/{attendanceID}")]
        public IActionResult ProcessCheckOut(int attendanceID)
        {
            var attendance = _unitOfWork.Attendance.Get(i => i.AttendanceID == attendanceID);
            if (attendance == null)
            {
                return NotFound();
            }

            attendance.AttendOut = DateTime.Now;
            _unitOfWork.Attendance.Update(attendance);
            _unitOfWork.Commit();
            return Json(new { attendanceID = attendance.AttendanceID, CheckedIn = attendance.AttendIn.ToString("hh:mm tt"), CheckedOut = attendance.AttendOut.Value.ToString("hh:mm tt") });
        }

        [HttpPost("CancelCheckIn/{attendanceID}")]
        public IActionResult CancelCheckIn(int attendanceID)
        {
            var attendance = _unitOfWork.Attendance.Get(i => i.AttendanceID == attendanceID);
            if(attendance == null)
            {
                return NotFound();
            }
            var enrollment = _unitOfWork.Enrollment.Get(i => i.EnrollmentID == attendance.EnrollmentID);
            _unitOfWork.Attendance.Delete(attendance);
            _unitOfWork.Commit();
            return Json(new { enrollmentID = enrollment.EnrollmentID });
        }

        [HttpPost("CancelCheckOut/{attendanceID}")]
        public IActionResult CancelCheckOut(int attendanceID)
        {
            var attendance = _unitOfWork.Attendance.Get(i => i.AttendanceID == attendanceID);
            if(attendance == null)
            {
                return NotFound();
            }
            attendance.AttendOut = null;
            _unitOfWork.Attendance.Update(attendance);
            _unitOfWork.Commit();
            return Json(new { AttendanceID = attendance.AttendanceID, CheckedIn = attendance.AttendIn.ToString("hh:mm tt") });
        }

        [HttpPost("UpdateMeal/{attendanceID}/{meal}")]
        public IActionResult UpdateMeal(int attendanceID, string meal)
        {
            var attendance = _unitOfWork.Attendance.Get(i => i.AttendanceID == attendanceID);
            if (attendance == null)
            {
                return Json(new { Status = $"{meal} not updated, attendance entry not found." });
            }
            if(meal.ToLower() == "breakfast")
            {
                attendance.HadBreakfast = !attendance.HadBreakfast;
            }
            else if(meal.ToLower() == "lunch")
            {
                attendance.HadLunch = !attendance.HadLunch;
            }
            else if(meal.ToLower() == "snack")
            {
                attendance.HadSnack = !attendance.HadSnack;
            }
            _unitOfWork.Attendance.Update(attendance);
            _unitOfWork.Commit();
            return Json(new { Status = $"{meal} updated successfully!" });
        }
    }
}
