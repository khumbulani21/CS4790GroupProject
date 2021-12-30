using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians.ContactInfos
{
    public class AddContactMethodModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ApplicationUser CurrentASPGuardian { get; set; }

        [BindProperty]
        public Child CurrentChild { get; set; }

        [BindProperty]
        public string CurrentASPGuardianID { get; set; }

        [BindProperty]
        public int CurrentChildID { get; set; }

        [BindProperty]
        public ContactInfo CurrentContactInfo { get; set; }

        [BindProperty]
        public ContactType CurrentContactType { get; set; }

        [BindProperty]
        public AuthorizedToPickUp CurrentAuthorizedToPickUp { get; set; }

        [BindProperty]
        public bool NewContactMethod { get; set; }

        [BindProperty]
        public bool AuthorizationValue { get; set; }

        [BindProperty]
        public IEnumerable<ContactType> ContactTypes { get; set; }


        public AddContactMethodModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;


        //public IActionResult OnGet(string GuardianID, int ChildID)
        public IActionResult OnGet(string GuardianID, int ChildID)
        {
            CurrentChildID = ChildID;
            CurrentASPGuardianID = GuardianID;

            //we get guardian aspnet user id because it is more encrypted
            // returns to homepage if there is no guardain logged in
            if (_unitOfWork.ApplicationUser.Get(a => a.Id == CurrentASPGuardianID) == null)
            {
                return Redirect("/");
            }

            // gets the current child and guardian
            CurrentASPGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == CurrentASPGuardianID);
            CurrentChild = _unitOfWork.Child.Get(c => c.ChildID == CurrentChildID);

            // gets the contact types to show them in the list
            ContactTypes = _unitOfWork.ContactType.List();

            CurrentContactInfo = new ContactInfo();
            CurrentContactType = new ContactType();
            CurrentAuthorizedToPickUp = new AuthorizedToPickUp();

            return Page();
        }

        public IActionResult OnPost()
        {
            DateTime currentTime = DateTime.Now;
            CurrentASPGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == CurrentASPGuardianID);
            CurrentChild = _unitOfWork.Child.Get(c => c.ChildID == CurrentChildID);


            // Add the new contact method
            CurrentContactInfo.GuardianID = Convert.ToInt32(CurrentASPGuardian.GuardianId);
            CurrentContactInfo.ChildID = CurrentChild.ChildID;
            CurrentContactInfo.ModifiedBy = CurrentASPGuardianID;
            CurrentContactInfo.ModifiedDate = currentTime;
            _unitOfWork.ContactInfo.Add(CurrentContactInfo);

            // if the contact is authorized to pick up
            if (AuthorizationValue)
            {
                CurrentAuthorizedToPickUp.ContactID = CurrentContactInfo.ContactID;
                CurrentAuthorizedToPickUp.ModifiedDate = currentTime;
                CurrentAuthorizedToPickUp.ModifiedBy = CurrentASPGuardianID;
                _unitOfWork.AuthorizedToPickUp.Add(CurrentAuthorizedToPickUp);
            }


            _unitOfWork.Commit();
            return Redirect("/Guardians/ContactInfos/Index/?GuardianID="+ CurrentASPGuardianID + "&ChildID=" + CurrentChildID);
        }
    }
}
