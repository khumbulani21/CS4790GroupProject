using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.ViewModel
{
    public class EmployeeVM
    {
        public string Id { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public List<string> Role { get; set; }
        public List<string> Locked { get; set; }
    }
}
