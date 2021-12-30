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
    public class UpdateContactMethodModel : PageModel
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


        public UpdateContactMethodModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;


        //public IActionResult OnGet(string GuardianID, int ChildID)
        public IActionResult OnGet(string GuardianID, int ContactID)
        {
            //we do not get a contact id that is in the contact info
            if (_unitOfWork.ContactInfo.Get(a => a.ContactID == ContactID) == null)
            {
                return Redirect("/");
            }

            CurrentContactInfo = _unitOfWork.ContactInfo.Get(i=>i.ContactID == ContactID);

            CurrentChildID = CurrentContactInfo.ChildID;
            CurrentASPGuardianID = GuardianID;
            CurrentGuardianID = CurrentContactInfo.GuardianID;

            // gets the current child and guardian
            CurrentASPGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == CurrentASPGuardianID);
            CurrentChild = _unitOfWork.Child.Get(c => c.ChildID == CurrentChildID);

            // gets the contact types to show them in the list
            ContactTypes = _unitOfWork.ContactType.List();

            // sets the corresponding Authorization type
            CurrentAuthorizedToPickUp = _unitOfWork.AuthorizedToPickUp.Get(a=> a.ContactID == CurrentContactInfo.ContactID);
            if(CurrentAuthorizedToPickUp != null)
            {
                AuthorizationValue = true;
            }


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
            _unitOfWork.ContactInfo.Update(CurrentContactInfo);

            // if the contact is authorized to pick up
            if (AuthorizationValue)
            {
                CurrentAuthorizedToPickUp = _unitOfWork.AuthorizedToPickUp.Get(a => a.ContactID == CurrentContactInfo.ContactID);
                
                // if there is no entry we need to add it
                if (CurrentAuthorizedToPickUp == null)
                {
                    CurrentAuthorizedToPickUp = new AuthorizedToPickUp();
                    CurrentAuthorizedToPickUp.ContactID = CurrentContactInfo.ContactID;
                    CurrentAuthorizedToPickUp.ModifiedDate = currentTime;
                    CurrentAuthorizedToPickUp.ModifiedBy = CurrentASPGuardianID;
                    _unitOfWork.AuthorizedToPickUp.Add(CurrentAuthorizedToPickUp);

                }
                // if not then we update it
                else
                {
                    CurrentAuthorizedToPickUp.ContactID = CurrentContactInfo.ContactID;
                    CurrentAuthorizedToPickUp.ModifiedDate = currentTime;
                    CurrentAuthorizedToPickUp.ModifiedBy = CurrentASPGuardianID;
                    _unitOfWork.AuthorizedToPickUp.Update(CurrentAuthorizedToPickUp);
                }

            }
            // if the selected not authorized and we had an authorized existing record we need to remove it
            else
            {
                CurrentAuthorizedToPickUp = _unitOfWork.AuthorizedToPickUp.Get(a => a.ContactID == CurrentContactInfo.ContactID);
                if (CurrentAuthorizedToPickUp != null)
                {
                    _unitOfWork.AuthorizedToPickUp.Delete(CurrentAuthorizedToPickUp);
                }
            }


            _unitOfWork.Commit();
            return Redirect("/Guardians/ContactInfos/Index/?GuardianID=" + CurrentASPGuardianID + "&ChildID=" + CurrentChildID);
        }
    }
}
