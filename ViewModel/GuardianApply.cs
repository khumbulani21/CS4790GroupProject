using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.ViewModel
{
    public class GuardianApply
    {
        public Application application { get; set; }

        public IEnumerable<SelectListItem> ChildList { get; set; }

    }
}
