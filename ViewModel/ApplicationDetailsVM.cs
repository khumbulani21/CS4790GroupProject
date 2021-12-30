using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.ViewModel
{
    public class ApplicationDetailsVM
    {
        public IEnumerable<Guardian> Guardians { get; set; }
        public IEnumerable<Child> Children { get; set; }
        public IEnumerable<Application> Applications { get; set; }

    }
}
