using ApplicationCore.Models;
using System.Collections.Generic;

namespace CS4790GroupProject.ViewModel
{
    public class AdminAbsenceViewModel
    {
        public string Date { get; set; }
        public List<Enrollment> Children { get; set; }
    }
}
