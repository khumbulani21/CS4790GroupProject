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

namespace CS4790GroupProject.Pages.Admin.TransactionTypes
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public TransactionType TransactionType { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int TransactionTypeID)
        {
            //checks if session is active and redirects to page to login again
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToPage("/SessionExpired");
            }

            //ensures that only the user in the guardian role can access this page
            if (!User.IsInRole(SD.AdminRole))
            {
                return Forbid();
            }

            if (TransactionTypeID != 0)
            {
                TransactionType = _unitOfWork.TransactionType.Get(i => i.TransTypeID == TransactionTypeID);
                if (TransactionType == null)
                {
                    return NotFound();
                }
            }
            else
            {
                TransactionType = new TransactionType();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            TransactionType.ModifiedBy = "Before Users";
            TransactionType.ModifiedDate = DateTime.Now;
            if (TransactionType.TransTypeID == 0)
            {
                _unitOfWork.TransactionType.Add(TransactionType);
            }
            else
            {
                _unitOfWork.TransactionType.Update(TransactionType);
            }
            _unitOfWork.Commit();
            return RedirectToPage("/Admin/TransactionTypes/Index");
        }
    }
}
