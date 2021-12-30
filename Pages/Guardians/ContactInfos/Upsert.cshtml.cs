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
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ApplicationUser currentASPGuardian { get; set; }

        [BindProperty]
        public Child currentChild { get; set; }

        [BindProperty]
        public string currentASPGuardianID { get; set; }

        [BindProperty]
        public int currentChildID { get; set; }

        [BindProperty]
        public ContactInfo contactInfoPrimaryPhone { get; set; }

        [BindProperty]
        public ContactType contactTypePrimaryPhone { get; set; }

        [BindProperty]
        public ContactInfo contactInfoPrimaryEmail { get; set; }

        [BindProperty]
        public ContactType contactTypePrimaryEmail { get; set; }

        [BindProperty]
        public ContactType contactType1Guardian1 { get; set; }

        [BindProperty]
        public ContactType contactType2Guardian1 { get; set; }

        [BindProperty]
        public ContactInfo contactInfo1Guardian1 { get; set; }

        [BindProperty]
        public ContactInfo contactInfo2Guardian1 { get; set; }

        [BindProperty]
        public ContactType contactType1Guardian2 { get; set; }

        [BindProperty]
        public ContactType contactType2Guardian2 { get; set; }

        [BindProperty]
        public ContactInfo contactInfo1Guardian2 { get; set; }

        [BindProperty]
        public ContactInfo contactInfo2Guardian2 { get; set; }

        [BindProperty]
        public ContactInfo AuthorizedPickUpContactInfo1 { get; set; }

        [BindProperty]
        public ContactType AuthorizedPickUpContactType1 { get; set; }

        [BindProperty]
        public ContactInfo AuthorizedPickUpContactInfo2 { get; set; }

        [BindProperty]
        public ContactType AuthorizedPickUpContactType2 { get; set; }

        [BindProperty]
        public ContactInfo AlternativePickUpContactInfo1 { get; set; }

        [BindProperty]
        public ContactType AlternativePickUpContactType1 { get; set; }

        [BindProperty]
        public ContactInfo AlternativePickUpContactInfo2 { get; set; }

        [BindProperty]
        public ContactType AlternativePickUpContactType2 { get; set; }

        // out of area authorized pick uper
        [BindProperty]
        public ContactInfo AuthorizedOOAContactInfo { get; set; }

        // out of area authorized pick uper
        [BindProperty]
        public ContactType AuthorizedOOAContactType { get; set; }

        [BindProperty]
        public string Auth1RelationshipVal { get; set; }

        [BindProperty]
        public string Auth2RelationshipVal { get; set; }

        [BindProperty]
        public string AuthOOARelationshipVal { get; set; }

        [BindProperty]
        public string Alt1RelationshipVal { get; set; }

        [BindProperty]
        public string Alt2RelationshipVal { get; set; }

        [BindProperty]
        public string DocResponseVal { get; set; }

        [BindProperty]
        public string DentistResponseVal { get; set; }

        [BindProperty]
        public string InsuranceResponseVal { get; set; }

        [BindProperty]
        public string HospitalResponseVal { get; set; }

        [BindProperty]
        public int DocContactID { get; set; }

        [BindProperty]
        public int DentistContactID { get; set; }

        [BindProperty]
        public int HospitalContactID { get; set; }

        [BindProperty]
        public int InsuranceContactID { get; set; }

        public SD SD { get; }

        public IEnumerable<EmergencyContact> EmergencyContactList { get; set; }

        public IEnumerable<ContactInfo> ContactInfoList { get; set; }

        [BindProperty]
        public bool hasGuardian2 { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;


        public IActionResult OnGet(string GuardianID, int ChildID)
        {
            currentASPGuardianID = GuardianID;
            currentChildID = ChildID;

            // if there is an entry for emergency card form, we return to home page
            EmergencyContactList = _unitOfWork.EmergencyContact.List(e => e.ChildID == currentChildID);
            if (!EmergencyContactList.Any())
            {
                return Redirect("/Guardians/Index/?id=" + currentASPGuardianID);
            }

            //we get guardian aspnet user id because it is more encrypted
            // returns to homepage if there is no guardain logged in
            if (_unitOfWork.ApplicationUser.Get(a => a.Id == currentASPGuardianID) == null)
            {
                return Redirect("/");
            }


            currentASPGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == currentASPGuardianID);
            currentChild = _unitOfWork.Child.Get(c => c.ChildID == currentChildID);

            contactInfo1Guardian2 = new ContactInfo();
            contactInfo2Guardian2 = new ContactInfo();
            contactType1Guardian2 = new ContactType();
            contactType2Guardian2 = new ContactType();

            ContactInfoList = _unitOfWork.ContactInfo.List(c=> c.ChildID == currentChildID);
            for(int i=0; i < ContactInfoList.Count(); i++)
            {
                if(ContactInfoList.ElementAt(i).PrimaryContact == true)
                {
                    ContactType type = new ContactType();
                    type = _unitOfWork.ContactType.Get(t => t.ContactTypeID == ContactInfoList.ElementAt(i).ContactTypeID);
                    if (type.ContactTypeDescription == SD.PrimaryPhone)
                    {
                        contactInfoPrimaryPhone = ContactInfoList.ElementAt(i);
                    }
                    if (type.ContactTypeDescription == SD.PrimaryEmail)
                    {
                        contactInfoPrimaryEmail = ContactInfoList.ElementAt(i);
                    }
                }

                // Gets the Authorized types of contact
                EmergencyContact ec = _unitOfWork.EmergencyContact.Get(e => e.ContactID == ContactInfoList.ElementAt(i).ContactID);
                if (ec !=null){
                    if(ec.Type == SD.AuthorizedPickUp)
                    {
                        if(AuthorizedPickUpContactInfo1 == null)
                        {
                            AuthorizedPickUpContactInfo1 = ContactInfoList.ElementAt(i);
                            Auth1RelationshipVal = ec.Relationship;
                        }
                        else
                        {
                            AuthorizedPickUpContactInfo2 = ContactInfoList.ElementAt(i);
                            Auth2RelationshipVal = ec.Relationship;
                        }
                    }
                    if(ec.Type == SD.LocalAlternativePickUp)
                    {
                        if (AlternativePickUpContactInfo1 == null)
                        {
                            AlternativePickUpContactInfo1 = ContactInfoList.ElementAt(i);
                            Alt1RelationshipVal = ec.Relationship;
                        }
                        else
                        {
                            AlternativePickUpContactInfo2 = ContactInfoList.ElementAt(i);
                            Alt2RelationshipVal = ec.Relationship;
                        }
                    }
                    if (ec.Type == SD.OutOfAreaAlternativeContact)
                    {
                        if (AuthorizedOOAContactInfo == null)
                        {
                            AuthorizedOOAContactInfo = ContactInfoList.ElementAt(i);
                            AuthOOARelationshipVal = ec.Relationship;
                        }
                    }
                    if (ec.Relationship == SD.Guardian1)
                    { 
                        if(ec.Type == SD.CellPhone)
                        {
                            contactInfo1Guardian1 = ContactInfoList.ElementAt(i);
                        }
                        if (ec.Type == SD.WorkPhone)
                        {
                            contactInfo2Guardian1 = ContactInfoList.ElementAt(i);
                        }
                    }
                    if (ec.Relationship == SD.Guardian2)
                    {
                        hasGuardian2 = true;
                        if (ec.Type == SD.CellPhone)
                        {
                            contactInfo1Guardian2 = ContactInfoList.ElementAt(i);
                        }
                        if (ec.Type == SD.WorkPhone)
                        {
                            contactInfo2Guardian2 = ContactInfoList.ElementAt(i);
                        }
                    }
                    ec = null;
                }
                else
                {
                    ContactType type = new ContactType();
                    type = _unitOfWork.ContactType.Get(t => t.ContactTypeID == ContactInfoList.ElementAt(i).ContactTypeID);

                    if(type.ContactTypeDescription == SD.DoctorInfo)
                    {
                        DocResponseVal = ContactInfoList.ElementAt(i).ContactValue;
                        DocContactID = ContactInfoList.ElementAt(i).ContactID;
                    }
                    if (type.ContactTypeDescription == SD.Insurance)
                    {
                        InsuranceResponseVal = ContactInfoList.ElementAt(i).ContactValue;
                        InsuranceContactID = ContactInfoList.ElementAt(i).ContactID;
                    }
                    if (type.ContactTypeDescription == SD.DentistInfo)
                    {
                        DentistResponseVal = ContactInfoList.ElementAt(i).ContactValue;
                        DentistContactID = ContactInfoList.ElementAt(i).ContactID;
                    }
                    if (type.ContactTypeDescription == SD.HospitalPreference)
                    {
                        HospitalResponseVal = ContactInfoList.ElementAt(i).ContactValue;
                        HospitalContactID = ContactInfoList.ElementAt(i).ContactID;
                    }
                }

            }

            return Page();
        }

        public IActionResult OnPost()
        {
            DateTime currentTime = DateTime.Now;
            currentASPGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == currentASPGuardianID);
            currentChild = _unitOfWork.Child.Get(c => c.ChildID == currentChildID);

            // Primary ContactInfoPhone
            contactInfoPrimaryPhone.ContactFirst = contactInfo1Guardian1.ContactFirst;
            contactInfoPrimaryPhone.ContactLast = contactInfo1Guardian1.ContactLast;
            contactInfoPrimaryPhone.ChildID = currentChild.ChildID;
            contactInfoPrimaryPhone.PrimaryContact = true;
            contactInfoPrimaryPhone.ModifiedBy = currentASPGuardian.Id;
            contactInfoPrimaryPhone.ModifiedDate = currentTime;
            
            // Primary ContactInfoPhone
            contactInfoPrimaryEmail.ContactFirst = contactInfo1Guardian1.ContactFirst;
            contactInfoPrimaryEmail.ContactLast = contactInfo1Guardian1.ContactLast;
            contactInfoPrimaryEmail.ChildID = currentChild.ChildID;
            contactInfoPrimaryEmail.PrimaryContact = true;
            contactInfoPrimaryEmail.ModifiedBy = currentASPGuardian.Id;
            contactInfoPrimaryEmail.ModifiedDate = currentTime;


            // Cell phone contact method for guardian1
            contactInfo1Guardian1.ModifiedBy = currentASPGuardianID;
            contactInfo1Guardian1.ModifiedDate = currentTime;
            contactInfo1Guardian1.ChildID = currentChildID;

            // Work phone contact method for guardian1
            contactInfo2Guardian1.ModifiedBy = currentASPGuardianID;
            contactInfo2Guardian1.ModifiedDate = currentTime;
            contactInfo2Guardian1.ContactFirst = contactInfo1Guardian1.ContactFirst;
            contactInfo2Guardian1.ContactLast = contactInfo1Guardian1.ContactLast;
            contactInfo2Guardian1.ChildID = currentChildID;

            // check if we have a 2nd Guardian
            if (contactInfo1Guardian2.ContactFirst != null && contactInfo1Guardian2.ContactFirst != "")
            {
                if (contactInfo1Guardian2.ContactFirst != null && contactInfo1Guardian2.ContactFirst != "")
                {
                    if (_unitOfWork.EmergencyContact.Get(e => e.ContactID == contactInfo2Guardian2.ContactID) != null)
                    
                    {
                        // Cell phone contact method for guardian2
                        contactInfo1Guardian2.ModifiedBy = currentASPGuardianID;
                        contactInfo1Guardian2.ModifiedDate = currentTime;
                        contactInfo1Guardian2.ChildID = currentChildID;
                        contactInfo1Guardian2.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
                        


                        // Work phone contact method for guardian2
                        contactInfo2Guardian2.ModifiedBy = currentASPGuardianID;
                        contactInfo2Guardian2.ModifiedDate = currentTime;
                        contactInfo2Guardian2.ContactFirst = contactInfo1Guardian2.ContactFirst;
                        contactInfo2Guardian2.ContactLast = contactInfo1Guardian2.ContactLast;
                        contactInfo2Guardian2.ChildID = currentChildID;
                    }
                    else
                    {
                        // Cell Phone
                        contactType1Guardian2.ModifiedBy = currentASPGuardianID;
                        contactType1Guardian2.ModifiedDate = currentTime;
                        contactInfo1Guardian2.ModifiedBy = currentASPGuardianID;
                        contactInfo1Guardian2.ModifiedDate = currentTime;
                        contactInfo1Guardian2.ChildID = currentChildID;
                        contactType1Guardian2.ContactTypeDescription = SD.CellPhone;


                        // Work phone contact method for guardian2
                        contactType2Guardian2.ModifiedBy = currentASPGuardianID;
                        contactType2Guardian2.ModifiedDate = currentTime;
                        contactInfo2Guardian2.ModifiedBy = currentASPGuardianID;
                        contactInfo2Guardian2.ModifiedDate = currentTime;
                        contactInfo2Guardian2.ContactFirst = contactInfo1Guardian2.ContactFirst;
                        contactInfo2Guardian2.ContactLast = contactInfo1Guardian2.ContactLast;
                        contactInfo2Guardian2.ChildID = currentChildID;
                        contactType2Guardian2.ContactTypeDescription = SD.WorkPhone;
                    }
                }
            }

            // authorized child pick upers 

            // authorized 1
            AuthorizedPickUpContactInfo1.ChildID = currentChildID;
            AuthorizedPickUpContactInfo1.ModifiedBy = currentASPGuardianID;
            AuthorizedPickUpContactInfo1.ModifiedDate = currentTime;

            // authorized 2
            AuthorizedPickUpContactInfo2.ChildID = currentChildID;
            AuthorizedPickUpContactInfo2.ModifiedBy = currentASPGuardianID;
            AuthorizedPickUpContactInfo2.ModifiedDate = currentTime;

            // alternative authorized child pick upers 

            // alternative authorized 1
            AlternativePickUpContactInfo1.ChildID = currentChildID;
            AlternativePickUpContactInfo1.ModifiedBy = currentASPGuardianID;
            AlternativePickUpContactInfo1.ModifiedDate = currentTime;


            // alternative authorized 2
            AlternativePickUpContactInfo2.ChildID = currentChildID;
            AlternativePickUpContactInfo2.ModifiedBy = currentASPGuardianID;
            AlternativePickUpContactInfo2.ModifiedDate = currentTime;

            // out of area alternative authorized 
            AuthorizedOOAContactInfo.ChildID = currentChildID;
            AuthorizedOOAContactInfo.ModifiedBy = currentASPGuardianID;
            AuthorizedOOAContactInfo.ModifiedDate = currentTime;
            AuthorizedOOAContactInfo.OutOfArea = true;


            if (contactInfo1Guardian2.ContactFirst != null && contactInfo1Guardian2.ContactFirst != "")
            {
                if (_unitOfWork.EmergencyContact.Get(e => e.ContactID == contactInfo2Guardian2.ContactID) != null)
                {

                }
                else
                {
                    _unitOfWork.ContactType.Add(contactType1Guardian2);
                    _unitOfWork.ContactType.Add(contactType2Guardian2);
                }

            }


            // guardian id for fk constraint
            contactInfoPrimaryPhone.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            contactInfoPrimaryEmail.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);

            contactInfo1Guardian1.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            contactInfo2Guardian1.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            contactInfo1Guardian2.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            contactInfo2Guardian2.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);

            AuthorizedPickUpContactInfo1.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            AuthorizedPickUpContactInfo2.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            AlternativePickUpContactInfo1.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            AlternativePickUpContactInfo2.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            AuthorizedOOAContactInfo.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);

            _unitOfWork.ContactInfo.Update(contactInfoPrimaryPhone);
            _unitOfWork.ContactInfo.Update(contactInfoPrimaryEmail);
            _unitOfWork.ContactInfo.Update(contactInfo1Guardian1);
            _unitOfWork.ContactInfo.Update(contactInfo2Guardian1);
            bool isUpdateG2 = false;
            if (contactInfo1Guardian2.ContactFirst != null && contactInfo1Guardian2.ContactFirst != "")
            {
                ContactInfo contactInfo = new ContactInfo();
                
                //contactInfo = _unitOfWork.ContactInfo.Get(i => i.ContactID == contactInfo1Guardian2.ContactID);
                if (_unitOfWork.EmergencyContact.Get(e => e.ContactID == contactInfo2Guardian2.ContactID) != null){

                    isUpdateG2 = true;
                }
                else
                {
                    contactInfo1Guardian2.ContactTypeID = contactType1Guardian2.ContactTypeID;
                    contactInfo2Guardian2.ContactTypeID = contactType2Guardian2.ContactTypeID;
                    
                    _unitOfWork.ContactInfo.Add(contactInfo1Guardian2);
                    _unitOfWork.ContactInfo.Add(contactInfo2Guardian2);
                }
            }
            if (isUpdateG2)
            {
                _unitOfWork.ContactInfo.Update(contactInfo2Guardian2);
                _unitOfWork.ContactInfo.Update(contactInfo1Guardian2);
                isUpdateG2 = false;
            }
            _unitOfWork.ContactInfo.Update(AuthorizedPickUpContactInfo1);
            _unitOfWork.ContactInfo.Update(AuthorizedPickUpContactInfo2);
            _unitOfWork.ContactInfo.Update(AlternativePickUpContactInfo1);
            _unitOfWork.ContactInfo.Update(AlternativePickUpContactInfo2);
            _unitOfWork.ContactInfo.Update(AuthorizedOOAContactInfo);

            // Creating the emergency contacts
            // authorized pick ups
            EmergencyContact auth1EC = new EmergencyContact();
            EmergencyContact auth2EC = new EmergencyContact();
            auth1EC = _unitOfWork.EmergencyContact.Get(e=>e.ContactID == AuthorizedPickUpContactInfo1.ContactID &&
                e.ChildID == currentChildID);
            auth2EC = _unitOfWork.EmergencyContact.Get(e => e.ContactID == AuthorizedPickUpContactInfo2.ContactID &&
                e.ChildID == currentChildID);

            auth1EC.ChildID = currentChildID;
            auth1EC.ContactID = AuthorizedPickUpContactInfo1.ContactID;
            auth1EC.Type = SD.AuthorizedPickUp;
            auth1EC.Relationship = Auth1RelationshipVal;
            auth1EC.ModifiedBy = currentASPGuardianID;
            auth1EC.ModifiedDate = currentTime;

            auth2EC.ChildID = currentChildID;
            auth2EC.ContactID = AuthorizedPickUpContactInfo2.ContactID;
            auth2EC.Type = SD.AuthorizedPickUp;
            auth2EC.Relationship = Auth2RelationshipVal;
            auth2EC.ModifiedBy = currentASPGuardianID;
            auth2EC.ModifiedDate = currentTime;

            _unitOfWork.EmergencyContact.Update(auth1EC);
            _unitOfWork.EmergencyContact.Update(auth2EC);

            // local alternative pick ups
            EmergencyContact alt1EC = new EmergencyContact();
            EmergencyContact alt2EC = new EmergencyContact();
            alt1EC = _unitOfWork.EmergencyContact.Get(e => e.ContactID == AlternativePickUpContactInfo1.ContactID &&
                e.ChildID == currentChildID);
            alt2EC = _unitOfWork.EmergencyContact.Get(e => e.ContactID == AlternativePickUpContactInfo2.ContactID &&
                e.ChildID == currentChildID);

            alt1EC.ChildID = currentChildID;
            alt1EC.ContactID = AlternativePickUpContactInfo1.ContactID;
            alt1EC.Type = SD.LocalAlternativePickUp;
            alt1EC.Relationship = Alt1RelationshipVal;
            alt1EC.ModifiedBy = currentASPGuardianID;
            alt1EC.ModifiedDate = currentTime;

            alt2EC.ChildID = currentChildID;
            alt2EC.ContactID = AlternativePickUpContactInfo2.ContactID;
            alt2EC.Type = SD.LocalAlternativePickUp;
            alt2EC.Relationship = Alt2RelationshipVal;
            alt2EC.ModifiedBy = currentASPGuardianID;
            alt2EC.ModifiedDate = currentTime;

            _unitOfWork.EmergencyContact.Update(alt1EC);
            _unitOfWork.EmergencyContact.Update(alt2EC);
            // out of area Emergency Contact
            EmergencyContact authOOAEC = new EmergencyContact();
            authOOAEC = _unitOfWork.EmergencyContact.Get(e => e.ContactID == AuthorizedOOAContactInfo.ContactID &&
                e.ChildID == currentChildID);

            authOOAEC.ChildID = currentChildID;
            authOOAEC.ContactID = AuthorizedOOAContactInfo.ContactID;
            authOOAEC.Type = SD.OutOfAreaAlternativeContact;
            authOOAEC.Relationship = AuthOOARelationshipVal;
            authOOAEC.ModifiedBy = currentASPGuardianID;
            authOOAEC.ModifiedDate = currentTime;

            _unitOfWork.EmergencyContact.Update(authOOAEC);

            // guardians
            EmergencyContact guardian1EC1 = new EmergencyContact();
            EmergencyContact guardian1EC2 = new EmergencyContact();
            guardian1EC1 = _unitOfWork.EmergencyContact.Get(e => e.ContactID == contactInfo1Guardian1.ContactID &&
                e.ChildID == currentChildID);
            guardian1EC2 = _unitOfWork.EmergencyContact.Get(e => e.ContactID == contactInfo2Guardian1.ContactID &&
            e.ChildID == currentChildID);

            guardian1EC1.ChildID = currentChildID;
            guardian1EC1.ContactID = contactInfo1Guardian1.ContactID;
            guardian1EC1.Type = SD.CellPhone;
            guardian1EC1.Relationship = SD.Guardian1;
            guardian1EC1.ModifiedBy = currentASPGuardianID;
            guardian1EC1.ModifiedDate = currentTime;

            _unitOfWork.EmergencyContact.Update(guardian1EC1);

            guardian1EC2.ChildID = currentChildID;
            guardian1EC2.ContactID = contactInfo2Guardian1.ContactID;
            guardian1EC2.Type = SD.WorkPhone;
            guardian1EC2.Relationship = SD.Guardian1;
            guardian1EC2.ModifiedBy = currentASPGuardianID;
            guardian1EC2.ModifiedDate = currentTime;
            
            _unitOfWork.EmergencyContact.Update(guardian1EC2);

            if (contactInfo1Guardian2.ContactFirst != null && contactInfo1Guardian2.ContactFirst != "")
            {
                if (contactInfo1Guardian2.ContactFirst != null && contactInfo1Guardian2.ContactFirst != "")
                {
                    if (_unitOfWork.EmergencyContact.Get(e => e.ContactID == contactInfo1Guardian2.ContactID &&
                            e.ChildID == currentChildID) != null)
                    {
                        EmergencyContact guardian2EC = new EmergencyContact();
                        guardian2EC = _unitOfWork.EmergencyContact.Get(e => e.ContactID == contactInfo1Guardian2.ContactID &&
                            e.ChildID == currentChildID);
                        guardian2EC.ChildID = currentChildID;
                        guardian2EC.ContactID = contactInfo1Guardian2.ContactID;
                        guardian2EC.Type = SD.CellPhone;
                        guardian2EC.Relationship = SD.Guardian2;
                        guardian2EC.ModifiedBy = currentASPGuardianID;
                        guardian2EC.ModifiedDate = currentTime;

                        _unitOfWork.EmergencyContact.Update(guardian2EC);


                        EmergencyContact guardian2EC2 = new EmergencyContact();
                        guardian2EC2 = _unitOfWork.EmergencyContact.Get(e => e.ContactID == contactInfo2Guardian2.ContactID &&
                            e.ChildID == currentChildID);
                        guardian2EC2.ChildID = currentChildID;
                        guardian2EC2.Relationship = SD.Guardian2;
                        guardian2EC2.ModifiedBy = currentASPGuardianID;
                        guardian2EC2.ModifiedDate = currentTime;
                        guardian2EC2.ContactID = contactInfo2Guardian2.ContactID;
                        guardian2EC2.Type = SD.WorkPhone;
                        _unitOfWork.EmergencyContact.Update(guardian2EC2);
                    }
                    else
                    {
                        EmergencyContact guardian2EC = new EmergencyContact();

                        guardian2EC.ChildID = currentChildID;
                        guardian2EC.ContactID = contactInfo1Guardian2.ContactID;
                        guardian2EC.Type = SD.CellPhone;
                        guardian2EC.Relationship = SD.Guardian2;
                        guardian2EC.ModifiedBy = currentASPGuardianID;
                        guardian2EC.ModifiedDate = currentTime;

                        _unitOfWork.EmergencyContact.Add(guardian2EC);

                        EmergencyContact guardian2EC2 = new EmergencyContact();

                        guardian2EC2.ChildID = currentChildID;
                        guardian2EC2.Relationship = SD.Guardian2;
                        guardian2EC2.ModifiedBy = currentASPGuardianID;
                        guardian2EC2.ModifiedDate = currentTime;
                        guardian2EC2.ContactID = contactInfo2Guardian2.ContactID;
                        guardian2EC2.Type = SD.WorkPhone;
                        _unitOfWork.EmergencyContact.Add(guardian2EC2);
                    }

                }

            }

            // docotor
            ContactInfo docContactInfo = new ContactInfo();
            docContactInfo = _unitOfWork.ContactInfo.Get(i => i.ContactID == DocContactID);

            docContactInfo.ContactValue = DocResponseVal;
            docContactInfo.ContactFirst = "";
            docContactInfo.ContactLast = "";
            docContactInfo.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            docContactInfo.ChildID = currentChildID;
            docContactInfo.ModifiedBy = currentASPGuardianID;
            docContactInfo.ModifiedDate = currentTime;
            _unitOfWork.ContactInfo.Update(docContactInfo);


            // hospital
            ContactInfo hospitalContactInfo = new ContactInfo();
            hospitalContactInfo = _unitOfWork.ContactInfo.Get(i => i.ContactID == HospitalContactID);

            hospitalContactInfo.ContactValue = HospitalResponseVal;
            hospitalContactInfo.ContactFirst = "";
            hospitalContactInfo.ContactLast = "";
            hospitalContactInfo.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            hospitalContactInfo.ChildID = currentChildID;
            hospitalContactInfo.ModifiedBy = currentASPGuardianID;
            hospitalContactInfo.ModifiedDate = currentTime;
            _unitOfWork.ContactInfo.Update(hospitalContactInfo);

            // insurance
            ContactInfo insuranceContactInfo = new ContactInfo();
            insuranceContactInfo = _unitOfWork.ContactInfo.Get(i => i.ContactID == InsuranceContactID);

            insuranceContactInfo.ContactValue = InsuranceResponseVal;
            insuranceContactInfo.ContactFirst = "";
            insuranceContactInfo.ContactLast = "";
            insuranceContactInfo.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            insuranceContactInfo.ChildID = currentChildID;
            insuranceContactInfo.ModifiedBy = currentASPGuardianID;
            insuranceContactInfo.ModifiedDate = currentTime;
            _unitOfWork.ContactInfo.Update(insuranceContactInfo);

            // dentist
            ContactInfo dentistContactInfo = new ContactInfo();
            dentistContactInfo = _unitOfWork.ContactInfo.Get(i => i.ContactID == DentistContactID);


            dentistContactInfo.ContactValue = DentistResponseVal;
            dentistContactInfo.ContactFirst = "";
            dentistContactInfo.ContactLast = "";
            dentistContactInfo.GuardianID = Convert.ToInt32(currentASPGuardian.GuardianId);
            dentistContactInfo.ChildID = currentChildID;
            dentistContactInfo.ModifiedBy = currentASPGuardianID;
            dentistContactInfo.ModifiedDate = currentTime;
            _unitOfWork.ContactInfo.Update(dentistContactInfo);


            _unitOfWork.Commit();
            // TODO COMMMIT
            return Redirect("/Guardians/Index/?id=" + currentASPGuardianID);
        }
    }
}

