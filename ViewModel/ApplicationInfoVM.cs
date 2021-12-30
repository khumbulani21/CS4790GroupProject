using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.ViewModel
{
    public class ApplicationInfoVM
    {
    

        public Enrollment Enrollment { get; set; }
        public Child Child { get; set; }
        public int Age { get; set; }
        public ApplicationCore.Models.Program Program { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }

        public IEnumerable<SelectListItem> ProgramList { get; set; }
        public IEnumerable<SelectListItem> HourBlocks { get; set; }
        public IEnumerable<SelectListItem> SemesterList { get; set; }

    }
}
