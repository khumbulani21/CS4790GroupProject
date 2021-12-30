using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.ViewModel
{
    public class GuardianVolunteerVM
    {
        public VolunteerOpportunity VolunteerOpportunity;

        public Volunteer Volunteer;

        [BindProperty]
        public IList<SelectListItem> Opportunity { get; set; }

        [BindProperty]
        public IList<SelectListItem> GuardiansVolunteered { get; set; }
    }
}
