using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [BindProperty]
        public string guardianID { get; set; }

        public Guardian Guardian { get; set; }

        [BindProperty]
        public bool hasChildren { get; set; }

        public List<Child> Children { get; set; }

        [BindProperty]
        public List<ContactInfo> OtherContacts { get; set; }

        public IEnumerable<ContactInfo> CurrentContactInfos { get; set; }

        public IEnumerable<Application> ChildApplications { get; set; }

        public IEnumerable<FamilyGroup> FamilyGroupList { get; set; }

        public IEnumerable<EmergencyContact> EmergencyContactList { get; set; }

        public IEnumerable<Application> Applications { get; set; }

        public List<int> HasContactsComplete { get; set; }

        public List<Application> ApplicationStatus { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public ChildsRoutine ChildsRoutineForm { get; set; }

        public ChildGoals ChildGoalsForm { get; set; }

        public GuidanceAndBehavior GuidanceBehaviorForm { get; set; }

        public HomeEnvironmentFamily HomeEnvironmentForm { get; set; }

        public HealthAssessment HealthAssessmentForm { get; set; }

        public int CompletedForms { get; set; }
        public int CompletedHealthForm { get; set; }

        [BindProperty]
        public Application application { get; set; }

        [BindProperty]
        public IEnumerable<ContactInfo> OutofAreaContacts { get; set; }

        [BindProperty]
        public IEnumerable<ContactInfo> PrimaryContacts { get; set; }

        [BindProperty]
        public List<ContactInfo> AuthorizedContactInfo { get; set; }

        [BindProperty]
        public EmergencyCardForm MedicalContacts { get; set; }

        [BindProperty]
        public List<ApplicationCore.Models.Program> program { get; set; }

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

            if (id == null)
            {
                return Redirect("/");
            }

            ApplicationUser = _unitOfWork.ApplicationUser.Get(a => a.Id == id);

            Guardian = _unitOfWork.Guardian.Get(g => g.GuardianID == ApplicationUser.GuardianId);

            FamilyGroupList = _unitOfWork.FamilyGroup.List(f => f.GuardianID == Guardian.GuardianID);

            //List of ChildID who have all contact information completed
            HasContactsComplete = new List<int>();

            Applications = Enumerable.Empty<Application>();

            program = _unitOfWork.Program.List().ToList();

            ApplicationStatus = new List<Application>();

            Children = new List<Child>();

            OtherContacts = new List<ContactInfo>();

            AuthorizedContactInfo = new List<ContactInfo>();

            if (FamilyGroupList.Any())
            {
                //FormsComplete = false;

                for (int i = 0; i < FamilyGroupList.Count(); i++)
                {
                    if (FamilyGroupList.ElementAt(i) == null)
                    {
                        hasChildren = false;
                        continue;
                    }
                    else
                    {
                        hasChildren = true;
                        Child temp = _unitOfWork.Child.Get(c => c.ChildID == FamilyGroupList.ElementAt(i).ChildID);
                        EmergencyContactList = _unitOfWork.EmergencyContact.List(c => c.ChildID == temp.ChildID);
                        Applications = _unitOfWork.Application.List(c => c.ChildID == temp.ChildID);
                        PrimaryContacts = _unitOfWork.ContactInfo.List(p => (p.ChildID == temp.ChildID) && (p.PrimaryContact == true));
                        OutofAreaContacts = _unitOfWork.ContactInfo.List(o => (o.ChildID == temp.ChildID) && (o.OutOfArea == true));
                        MedicalContacts = _unitOfWork.EmergencyCardForm.Get(e => e.ChildID == temp.ChildID);
                        CurrentContactInfos = _unitOfWork.ContactInfo.List(i => i.ChildID == temp.ChildID);
                        application = _unitOfWork.Application.Get(c => c.ChildID == temp.ChildID);

                        //if the child has applications add them to the ApplicationStatus list
                        if (Applications.Any())
                        {
                            foreach (var app in Applications)
                            {
                                ApplicationStatus.Add(app);
                            }

                            //if application exists, check if forms have been completed
                            ChildsRoutineForm = _unitOfWork.ChildsRoutine.Get(c => c.ApplicationID == application.ApplicationID);
                            ChildGoalsForm = _unitOfWork.ChildGoals.Get(c => c.ApplicationID == application.ApplicationID);
                            GuidanceBehaviorForm = _unitOfWork.GuidanceAndBehavior.Get(c => c.ApplicationID == application.ApplicationID);
                            HomeEnvironmentForm = _unitOfWork.HomeEnvironmentFamily.Get(c => c.ApplicationID == application.ApplicationID);

                            if ((ChildsRoutineForm != null) && (ChildGoalsForm != null) && (GuidanceBehaviorForm != null) && (HomeEnvironmentForm != null))
                            {
                                CompletedForms = 1;
                                HealthAssessmentForm = _unitOfWork.HealthAssessment.Get(h => h.ApplicationID == application.ApplicationID);
                            }

                        }


                        for (int j = 0; j < CurrentContactInfos.Count(); j++)
                        {
                            // If this contact method is in authorized to pick up we will add to authorized list
                            if (_unitOfWork.AuthorizedToPickUp.Get(a => a.ContactID == CurrentContactInfos.ElementAt(j).ContactID) != null)
                            {
                                AuthorizedContactInfo.Add(CurrentContactInfos.ElementAt(j));
                            }

                            else if (!PrimaryContacts.Contains(CurrentContactInfos.ElementAt(j)) &&
                                !OutofAreaContacts.Contains(CurrentContactInfos.ElementAt(j)))
                            {
                                OtherContacts.Add(CurrentContactInfos.ElementAt(j));
                            }
                        }

                        //if contacts are complete, add ChildId to list
                        if (OutofAreaContacts.Any() && PrimaryContacts.Any() && AuthorizedContactInfo.Any() && MedicalContacts != null)
                        {
                            HasContactsComplete.Add(temp.ChildID);
                        }

                        Children.Add(temp);
                    }
                }
            }

            guardianID = id;
            if (HealthAssessmentForm != null)
            {
                return Redirect("/Guardians/Home?id=" + guardianID);
            }
            else
            {
                return Page();
            }

        }
    }
}
