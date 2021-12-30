using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private List<ProgramSemesters> programSemesters;

        public ProgramReportController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        
        [HttpGet]
        public IActionResult Get()
        {
            var programs = _unitOfWork.Program.List(null, null, "Enrollments.Semester");
            
            programSemesters = new List<ProgramSemesters>();

            // gets the distinct list of all the programs and their specific semesters
            foreach (var program in programs)
            {
                programSemesters.AddRange(program.Enrollments.Select(x => x.Semester).Distinct()
                    .Select(x => new ProgramSemesters { ProgramID = program.ProgramID, SemesterID = x.SemesterID, 
                        SemesterName = x.SemesterName, ProgramName = program.ProgramName })
                    .ToList());
            }

            // we grab the enrollment id from those specific programs to use to pass and we can grab the
            // semester id from it and program id
            foreach(var ps in programSemesters)
            {
                Enrollment enrollment = _unitOfWork.Enrollment.Get(e => e.ProgramID == ps.ProgramID && e.SemesterID == ps.SemesterID);
                ps.EnrollmentID = enrollment.EnrollmentID;
            }
            
            return Json(new { data = programSemesters });
        }
        public class ProgramSemesters
        {
            public int ProgramID { get; set; }
            public int SemesterID { get; set; }
            public int EnrollmentID { get; set; }
            public string ProgramName { get; set; }
            public string SemesterName { get; set; }
        }
    }
}
