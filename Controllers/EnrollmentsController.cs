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
    public class EnrollmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public EnrollmentsController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        [HttpGet]
        public IActionResult Get()
        {


            var attendees = from e in _unitOfWork.Enrollment.List()

                            join a in _unitOfWork.Application.List()
                            on e.ApplicationID equals a.ApplicationID
                            join c in _unitOfWork.Child.List()
                            on a.ChildID equals c.ChildID
                            join s in _unitOfWork.Semester.List()
                            on e.SemesterID equals s.SemesterID
                            join p in _unitOfWork.Program.List()
                             on e.ProgramID equals p.ProgramID
                            
                            into Group
                            from p in Group.DefaultIfEmpty()

                            select new
                            {
                                ChildId = c.ChildID,
                                ChildFirst = c.ChildFirst,
                                ChildLast = c.ChildLast,
                                ChildDOB = c.ChildDOB,
                                ChildGender = c.ChildGender,
                                ChildStatus = c.ChildStatus,
                                ProgramID = a.ProgramID,
                                ProgramName = p.ProgramName,
                                SemesterName = s.SemesterName,
                                HourBlock = e.HourBlock,
                                EnrollmentID = e.EnrollmentID,
                                AppIdEnrollID = new string[] { a.ApplicationID.ToString(), e.EnrollmentID.ToString() }
                            };
            //attendees.Distinct()
            return Json(new { data = attendees });

        }


        [HttpGet("{applicationID}/{semesterID}")]
        public IActionResult Get(int applicationID, int semesterID)
        {
            //get all enrollments 
            if (applicationID==0)
            {
                return Json(new { data = 1 });
            }
            var enrollments = _unitOfWork.Enrollment.Get(e => e.ApplicationID == applicationID && e.SemesterID == semesterID);
            if (enrollments == null)
            {
                return Json(new { data = 0 });
            }
            else
            {
                return Json(new { data = 1 });
            }

        }
        [HttpGet("{enrollmentID}/{semesterID}/{newSemester}")]
        public IActionResult Get(int enrollmentID, int semesterID, int newSemester)
        {
            //get all enrollments 
            if (semesterID!=newSemester)
            {
                Enrollment enrollment = _unitOfWork.Enrollment.Get(e=>e.EnrollmentID==enrollmentID);
                var enrollments = _unitOfWork.Enrollment.Get(e => e.ApplicationID == enrollment.ApplicationID && e.SemesterID == newSemester);
                if (enrollments == null)
                {
                    return Json(new { data = 0 });
                }
                else
                {
                    return Json(new { data = 1 });
                }
            }
            else
            {
                return Json(new { data = 0 });
            }
            

        }
        [HttpDelete("{id}")]
        public    IActionResult Delete(int id)
        {

            Enrollment enrollment = _unitOfWork.Enrollment.Get(e => e.EnrollmentID == id);
            if (enrollment!=null)
            {
                _unitOfWork.Enrollment.Delete(enrollment);
                _unitOfWork.Commit();
            }
            else

            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            

            return Json(new { success = true, message = "Enrollement with ID: "+enrollment.EnrollmentID + " has been deleted " });
        }
       



    }
}
