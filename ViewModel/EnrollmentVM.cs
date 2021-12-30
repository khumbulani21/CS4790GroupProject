using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.ViewModel
{
    public class EnrollmentVM
    {
        public Enrollment Enrollment { get; set; }
        public Application Application { get; set; }
        public Child Child { get; set; }
        public Semester Semester { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
        public IEnumerable<SelectListItem> ApplicationList { get; set; }
        public IEnumerable<SelectListItem> ProgramList { get; set; }
        public IEnumerable<SelectListItem> HourBlocks { get; set; }
        public IEnumerable<SelectListItem> SemesterList { get; set; }
    }
}
