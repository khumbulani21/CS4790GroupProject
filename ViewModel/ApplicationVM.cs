using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.ViewModel
{
    public class ApplicationVM
    {
        public Semester Semester { get; set; }
        public IEnumerable<SelectListItem> SemesterList { get; set; }

    }
}
