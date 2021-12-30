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
    public class EnrollmentCountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

 
        public EnrollmentCountController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get(int semesterID, int programID,int applicationID)
        {
            IEnumerable<Enrollment> enrollments = _unitOfWork.Enrollment.List(e=>e.SemesterID==semesterID&& e.ProgramID==programID);
            
           ApplicationCore.Models.Program program = _unitOfWork.Program.Get(p=>p.ProgramID==programID);
            int capacity = 0;
            if (program!=null)
            {
                capacity = program.ProgramCapacity;
            }

            int numberOfEnrollments=enrollments.Count();
            var duplicateEnrollment = _unitOfWork.Enrollment.Get(e => e.ApplicationID == applicationID && e.SemesterID == semesterID);
            //check for duplicate enrollment
            int duplicateEnrollmentCount = 0;
            if (duplicateEnrollment != null)
            {
                duplicateEnrollmentCount = 1;
            }
            return Json(new { count = numberOfEnrollments,programCapacity=capacity, duplicateEnrollmentCount= duplicateEnrollmentCount });

        }

    }
}
