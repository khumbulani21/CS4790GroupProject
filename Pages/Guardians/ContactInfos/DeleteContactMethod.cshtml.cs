using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians.ContactInfos
{
    public class DeleteContactMethodModel : PageModel
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
        public int CurrentGuardianID { get; set; }

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


        public DeleteContactMethodModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;


        //public IActionResult OnGet(string GuardianID, int ChildID)
        public IActionResult OnGet(string GuardianID, int ContactID)
        {
            //we do not get a contact id that is in the contact info
            if (_unitOfWork.ContactInfo.Get(a => a.ContactID == ContactID) == null)
            {
                return Redirect("/");
            }

            CurrentContactInfo = _unitOfWork.ContactInfo.Get(i => i.ContactID == ContactID);

            CurrentChildID = CurrentContactInfo.ChildID;
            CurrentASPGuardianID = GuardianID;
            CurrentGuardianID = CurrentContactInfo.GuardianID;

            // gets the current child and guardian
            CurrentASPGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == CurrentASPGuardianID);
            CurrentChild = _unitOfWork.Child.Get(c => c.ChildID == CurrentChildID);

            // gets the contact types to show them in the list
            ContactTypes = _unitOfWork.ContactType.List();

            // sets the corresponding Authorization type
            CurrentAuthorizedToPickUp = _unitOfWork.AuthorizedToPickUp.Get(a => a.ContactID == CurrentContactInfo.ContactID);
            if (CurrentAuthorizedToPickUp != null)
            {
                AuthorizationValue = true;
            }


            return Page();
        }

        public IActionResult OnPost()
        {

            CurrentAuthorizedToPickUp = _unitOfWork.AuthorizedToPickUp.Get(a => a.ContactID == CurrentContactInfo.ContactID);
            if (CurrentAuthorizedToPickUp != null)
            {
                _unitOfWork.AuthorizedToPickUp.Delete(CurrentAuthorizedToPickUp);
            }
            _unitOfWork.ContactInfo.Delete(CurrentContactInfo);
            _unitOfWork.Commit();
            return Redirect("/Guardians/ContactInfos/Index/?GuardianID=" + CurrentASPGuardianID + "&ChildID=" + CurrentChildID);
        }
    }
}
