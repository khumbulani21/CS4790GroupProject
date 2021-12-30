using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.ViewModel;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CS4790GroupProject.Pages.Guardians
{
    public class VolunteerOpportunityModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public VolunteerOpportunityModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public GuardianVolunteerVM GuardianObj { get; set; }

        [TempData]
        public string SelectedOpportunity { get; set; }

        [TempData]
        public string SelectedOpportunityId { get; set; }

        [BindProperty]
        public ApplicationUser currentGuardian { get; set; }

        public IActionResult OnGet(string id)
        {
            //checks if session is active and redirects to page to login again
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToPage("/SessionExpired");
            }

            //ensures that only the user in the guardian role can access this page
            if (!User.IsInRole(SD.GuardianRole))
            {
                return Forbid();
            }
            //id = 9;

            // we get guardian aspnet user id because it is more encrypted
            // returns to homepage if there is no guardain logged in
            if (_unitOfWork.ApplicationUser.Get(a => a.Id == id) == null)
            {
                return Redirect("/");
            }

            currentGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == id);

            List<VolunteerOpportunity> op = new List<VolunteerOpportunity>();
            op = _unitOfWork.VolunteerOpportunity.List().ToList();
            List<Volunteer> vol = new List<Volunteer>();
            vol = _unitOfWork.Volunteer.List(c => c.GuardianID == currentGuardian.GuardianId).ToList();


            GuardianObj = new GuardianVolunteerVM
            {
                Opportunity = op.Select(c => new SelectListItem { Text = c.VolunteerOpDescription, Value = c.VolunteerOpID.ToString() })
                .ToList<SelectListItem>(),

                GuardiansVolunteered = vol.ToList<Volunteer>()
                .Select(c => new SelectListItem { Text = c.VolunteerOpportunity.VolunteerOpDescription, Value = c.VolunteerOpID.ToString() })
                .ToList<SelectListItem>()
            };

            return Page();

        }

        public IActionResult OnPost(string id)
        {
            //id = 9; //hard coding for now
            currentGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == id);

            if (GuardianObj.Opportunity != null)
            {
                foreach (SelectListItem item in GuardianObj.Opportunity)
                {
                    if (item.Selected)
                    {
                        SelectedOpportunity = $"{item.Text}, {SelectedOpportunity}";
                        SelectedOpportunityId = $"{item.Value}, {SelectedOpportunityId}";

                        Volunteer newVolunteer = new Volunteer();
                        newVolunteer.GuardianID = (int)currentGuardian.GuardianId; //hard coding for now until we create the log ins
                        newVolunteer.VolunteerOpID = Int32.Parse(item.Value);

                        _unitOfWork.Volunteer.Add(newVolunteer);

                        SelectedOpportunity = SelectedOpportunity.TrimEnd(',');
                        SelectedOpportunityId = SelectedOpportunityId.TrimEnd(',');
                    }
                }
            }

            if (GuardianObj.GuardiansVolunteered != null)
            {
                foreach (SelectListItem item in GuardianObj.GuardiansVolunteered)
                {

                    if (item.Selected)
                    {
                        SelectedOpportunity = $"{item.Text}, {SelectedOpportunity}";
                        SelectedOpportunityId = $"{item.Value}, {SelectedOpportunityId}";

                        Volunteer oldVolunteer = new Volunteer();
                        oldVolunteer = _unitOfWork.Volunteer.Get(c => c.GuardianID == currentGuardian.GuardianId
                        && c.VolunteerOpID.ToString() == item.Value);


                        _unitOfWork.Volunteer.Delete(oldVolunteer);
                        SelectedOpportunity = SelectedOpportunity.TrimEnd(',');
                        SelectedOpportunityId = SelectedOpportunityId.TrimEnd(',');
                    }
                }
            }

            _unitOfWork.Commit();

            return RedirectToPage("./VolunteerOpportunity", new { id = id });
        }
    }
}
