using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS4790GroupProject.Pages.Guardians.Addresses
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public ApplicationUser currentASPGuardian { get; set; }

        [BindProperty]
        public Guardian currentGuardian { get; set; }

        [BindProperty]
        public Address currentAddress { get; set; }



        public IndexModel(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


            public IActionResult OnGet(string id)
        {
            //checks if User Identity exists and redirects if session expired
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

            // we get guardian aspnet user id because it is more encrypted
            // returns to homepage if there is no guardain logged in
            if (_unitOfWork.ApplicationUser.Get(a => a.Id == id) == null)
            {
                return Redirect("/");
            }

            currentASPGuardian = _unitOfWork.ApplicationUser.Get(a => a.Id == id);
            currentGuardian = _unitOfWork.Guardian.Get(g => g.GuardianID == currentASPGuardian.GuardianId);

            currentAddress = _unitOfWork.Address.Get(a=> a.AddressID == currentGuardian.AddressID);

            return Page();
        }

        public IActionResult OnPost()
        {
            currentAddress.ModifiedBy = currentASPGuardian.Id;
            currentAddress.ModifiedDate = DateTime.Now;
            currentGuardian.ModifiedBy = currentASPGuardian.Id;
            currentGuardian.ModifiedDate = DateTime.Now;

            var User = _unitOfWork.ApplicationUser.Get(a => a.Id == currentASPGuardian.Id);
            User.PhoneNumber = currentASPGuardian.PhoneNumber;
            User.Email = currentASPGuardian.Email;
            User.WeberNumber = currentASPGuardian.WeberNumber;

            _unitOfWork.ApplicationUser.Update(User);
            _unitOfWork.Address.Update(currentAddress);
            _unitOfWork.Guardian.Update(currentGuardian);

            _unitOfWork.Commit();
            return Page();
        }
    }
}
