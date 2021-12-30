using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.ViewModel
{
    public class MultipleEnrollmentsVM
    {
        public int ProgramID { get; set; }
        public int SemesterID { get; set; }
        public IEnumerable<SelectListItem> ProgramList { get; set; }
        public IEnumerable<SelectListItem> SemesterList { get; set; }
    }
}
