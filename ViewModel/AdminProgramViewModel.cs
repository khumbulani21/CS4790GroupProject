using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.ViewModel
{
    public class AdminProgramViewModel
    {
        public ApplicationCore.Models.Program Program { get; set; }
        public int PendingApplications { get; set; }
        public int WaitlistedApplications { get; set; }
    }
}
