using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians.ContactInfos
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public IEnumerable<ContactType> ContactTypes { get; set; }

        [BindProperty]
        public IEnumerable<ContactInfo> PrimaryContacts { get; set; }

        [BindProperty]
        public List<ContactInfo> AuthorizedContactInfo { get; set; }

        [BindProperty]
        public List<ContactInfo> OtherContacts { get; set; }

        public IEnumerable<ContactInfo> CurrentContactInfos { get; set; }

        [BindProperty]
        public EmergencyCardForm MedicalContacts { get; set; }

        [BindProperty]
        public bool HasMedicalContact { get; set; }

        [BindProperty]
        public IEnumerable<ContactInfo> OutofAreaContacts { get; set; }

        [BindProperty]
        public int ProgressAmount { get; set; }

        [BindProperty]
        public int StepAmount { get; set; }

        [BindProperty]
        public string ProgressAmountStr { get; set; }

        public int CurrentChildID { get; set; }
        public string CurrentASPGuardianID { get; set; }


        public IndexModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(string GuardianID, int ChildID, bool? updateMode)
        {
            CurrentChildID = ChildID;
            CurrentASPGuardianID = GuardianID;

            ContactTypes = _unitOfWork.ContactType.List();

            PrimaryContacts = _unitOfWork.ContactInfo.List(p => (p.ChildID == CurrentChildID) && (p.PrimaryContact == true));
            OutofAreaContacts = _unitOfWork.ContactInfo.List(o => (o.ChildID == CurrentChildID) && (o.OutOfArea == true));
            MedicalContacts = _unitOfWork.EmergencyCardForm.Get(e => e.ChildID == ChildID);
            CurrentContactInfos = _unitOfWork.ContactInfo.List(i => i.ChildID == CurrentChildID);


            AuthorizedContactInfo = new List<ContactInfo>();
            OtherContacts = new List<ContactInfo>();

            for (int i = 0; i < CurrentContactInfos.Count(); i++)
            {
                // If this contact method is in authorized to pick up we will add to authorized list
                if (_unitOfWork.AuthorizedToPickUp.Get(a => a.ContactID == CurrentContactInfos.ElementAt(i).ContactID) != null)
                {
                    AuthorizedContactInfo.Add(CurrentContactInfos.ElementAt(i));
                }

                else if (!PrimaryContacts.Contains(CurrentContactInfos.ElementAt(i)) &&
                    !OutofAreaContacts.Contains(CurrentContactInfos.ElementAt(i)))
                {
                    OtherContacts.Add(CurrentContactInfos.ElementAt(i));
                }
            }

            if (OutofAreaContacts.Any())
            {
                ProgressAmount += 25;
                StepAmount += 1;
            }

            if (PrimaryContacts.Any())
            {
                ProgressAmount += 25;
                StepAmount += 1;
            }

            if (AuthorizedContactInfo.Any())
            {
                ProgressAmount += 25;
                StepAmount += 1;
            }

            if (MedicalContacts != null)
            {
                ProgressAmount += 25;
                StepAmount += 1;
                HasMedicalContact = true;
            }
            else
            {
                HasMedicalContact = false;
            }

            if (HasMedicalContact == true && ProgressAmount == 100)
            {
                if(updateMode != null && updateMode == false)
                {
                    return RedirectToPage("/Guardians/Index");
                }
                else
                {
                    ProgressAmountStr = ProgressAmount + "%";
                    return Page();
                }
            }
            else
            {
                ProgressAmountStr = ProgressAmount + "%";
                return Page();
            }


        }
    }
}

