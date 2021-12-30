using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians.ContactInfos
{
    public class MedicalContactModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public EmergencyCardForm CurrentEmergencyCardForm { get; set; }

        [BindProperty]
        public int CurrentChildID { get; set; }

        [BindProperty]
        public string CurrentASPGuardianID { get; set; }


        public MedicalContactModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(string GuardianID, int ChildID)
        {
            //checks if session is active and redirects to page to login again
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToPage("/SessionExpired");
            }
            CurrentChildID = ChildID;
            CurrentASPGuardianID = GuardianID;

            //we get guardian aspnet user id because it is more encrypted
            // returns to homepage if there is no guardain logged in
            if (_unitOfWork.ApplicationUser.Get(a => a.Id == CurrentASPGuardianID) == null)
            {
                return Redirect("/");
            }
            
           
            CurrentEmergencyCardForm = _unitOfWork.EmergencyCardForm.Get(e=>e.ChildID == CurrentChildID);
            if(CurrentEmergencyCardForm != null)
            {

            }
            else
            {
                CurrentEmergencyCardForm = new EmergencyCardForm();
            }


            return Page();
        }

        public IActionResult OnPost()
        {
            DateTime currentTime = DateTime.Now;
            CurrentEmergencyCardForm.FormID = 3;
            CurrentEmergencyCardForm.ChildID = CurrentChildID;
            CurrentEmergencyCardForm.ModifiedBy = CurrentASPGuardianID;

            if(CurrentEmergencyCardForm.EmergencyCardFormID == 0)
            {
                _unitOfWork.EmergencyCardForm.Add(CurrentEmergencyCardForm);
            }
            else
            {
                _unitOfWork.EmergencyCardForm.Update(CurrentEmergencyCardForm);
            }

            _unitOfWork.Commit();
            return Redirect("/Guardians/ContactInfos/Index/?GuardianID=" + CurrentASPGuardianID + "&ChildID=" + CurrentChildID);
        }
    }
}
