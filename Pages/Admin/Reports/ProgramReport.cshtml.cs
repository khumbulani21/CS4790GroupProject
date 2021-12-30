using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Admin.Reports
{
    public class ProgramReportModel : PageModel
    {
        public IUnitOfWork _unitOfWork { get; set; }

        [BindProperty]
        public ApplicationCore.Models.Program CurrentProgram { get; set; }

        [BindProperty]
        public ApplicationCore.Models.Semester CurrentSemester { get; set; }

        public IEnumerable<Enrollment> EnrollmentList { get; set; }

        public Enrollment CurrentEnrollment { get; set; }

        public int CurrentProgramID { get; set; }

        public int CurrentSemesterID { get; set; }

        public List<ApplicationCore.Models.Child> ListOfChildren { get; set; }

        public List<Application> ListOfApplications { get; set; }

        [BindProperty]
        public int MaleRatioAmount { get; set; }

        [BindProperty]
        public int FemaleRatioAmount { get; set; }

        [BindProperty]
        public int OtherGenderRatioAmount { get; set; }

        [BindProperty]
        public int PreferNotToSayGenderRatioAmount { get; set; }


        [BindProperty]
        public int NativeRatioAmount { get; set; }
        [BindProperty]
        public int AsianRatioAmount { get; set; }
        [BindProperty]
        public int BlackRatioAmount { get; set; }
        [BindProperty]
        public int PacificIslanderRatioAmount { get; set; }
        [BindProperty]
        public int WhiteRatioAmount { get; set; }
        [BindProperty]
        public int OtherRatioAmount { get; set; }
        [BindProperty]
        public int PreferNotToRespondRatioAmount { get; set; }


        public int HispanicorLatinoRatioAmount { get; set; }
        [BindProperty]
        public int NotHispanicorLatinoRatioAmount { get; set; }
        [BindProperty]
        public int PreferNotToRespondEthnicityRatioAmount { get; set; }


        public ProgramReportModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            CurrentEnrollment = new Enrollment();
            CurrentEnrollment = _unitOfWork.Enrollment.Get(e => e.EnrollmentID == id);
            CurrentProgramID = CurrentEnrollment.ProgramID;
            CurrentSemesterID = CurrentEnrollment.SemesterID;


            ListOfChildren = new List<ApplicationCore.Models.Child>();
            ListOfApplications = new List<Application>();
            CurrentProgram = _unitOfWork.Program.Get(p=> p.ProgramID == CurrentProgramID);
            CurrentSemester = _unitOfWork.Semester.Get(s => s.SemesterID == CurrentSemesterID);
            EnrollmentList = _unitOfWork.Enrollment.List(e=> e.ProgramID == CurrentProgramID && e.SemesterID == CurrentSemesterID);
            
            // Get all applications that are for the specific program
            for(int i = 0; i < EnrollmentList.Count(); i++)
            {
                Application nextApplication = new Application();
                nextApplication = _unitOfWork.Application.Get(a => a.ApplicationID == EnrollmentList.ElementAt(i).ApplicationID && a.ProgramID == CurrentProgramID);
                
                if(nextApplication != null)
                {
                    ListOfApplications.Add(nextApplication);
                }
            }

            // Get all children who are on those applications
            for (int i = 0; i < ListOfApplications.Count; i++)
            {
                ApplicationCore.Models.Child nextChild = new ApplicationCore.Models.Child();
                nextChild = _unitOfWork.Child.Get(c => c.ChildID == ListOfApplications.ElementAt(i).ChildID);
                
                if(nextChild != null)
                {
                    ListOfChildren.Add(nextChild);
                }
            }

            // Intialize the count of children's races
            NativeRatioAmount = 0;
            AsianRatioAmount = 0;
            BlackRatioAmount = 0;
            PacificIslanderRatioAmount = 0;
            WhiteRatioAmount = 0;
            OtherRatioAmount = 0;
            PreferNotToRespondRatioAmount = 0;

            // Intialize the count of children's genders
            OtherGenderRatioAmount = 0;
            PreferNotToSayGenderRatioAmount = 0;
            MaleRatioAmount = 0;
            FemaleRatioAmount = 0;

            // Intialize the count of children's ethnicity
            PreferNotToRespondEthnicityRatioAmount = 0;
            NotHispanicorLatinoRatioAmount = 0;
            HispanicorLatinoRatioAmount = 0;

            // Will increment the races of the given children for the program
            for (int i = 0; i < ListOfChildren.Count; i++)
            {
                ApplicationCore.Models.Child nextChild = new ApplicationCore.Models.Child(); 
                nextChild = ListOfChildren.ElementAt(i);
                if(nextChild != null)
                {
                    // Increment childrens race diversity amount
                    switch (nextChild.ChildRace)
                    {
                        case SD.AmericanIndianorAlaskaNative:
                            NativeRatioAmount++;
                            break;
                        case SD.Asian:
                            AsianRatioAmount++;
                            break;
                        case SD.BlackorAfricanAmerican:
                            BlackRatioAmount++;
                            break;
                        case SD.NativeHawaiianorOtherPacificIslander:
                            PacificIslanderRatioAmount++;
                            break;
                        case SD.White:
                            WhiteRatioAmount++;
                            break;
                        case SD.Other:
                            OtherRatioAmount++;
                            break;
                        case SD.PreferNotToRespond:
                            PreferNotToRespondRatioAmount++;
                            break;
                        default:
                            break;
                    }

                    // Increment childrens gender diversity amount
                    switch (nextChild.ChildGender)
                    {
                        case SD.MaleChar:
                            MaleRatioAmount++;
                            break;
                        case SD.FemaleChar:
                            FemaleRatioAmount++;
                            break;
                        case SD.OtherChar:
                            OtherGenderRatioAmount++;
                            break;
                        case SD.PreferNotToRespondChar:
                            PreferNotToSayGenderRatioAmount++;
                            break;
                        default:
                            break;
                    }

                    // Increment childrens ethnicity diversity amount
                    switch (nextChild.ChildEthnicity)
                    {
                        case SD.NotHispanicorLatino:
                            NotHispanicorLatinoRatioAmount++;
                            break;
                        case SD.HispanicorLatino:
                            HispanicorLatinoRatioAmount++;
                            break;
                        case SD.PreferNotToRespond:
                            PreferNotToRespondEthnicityRatioAmount++;
                            break;
                        default:
                            break;
                    }
                }
            }


            return Page();
        }
    }
}
