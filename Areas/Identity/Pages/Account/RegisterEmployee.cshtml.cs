using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.Areas.Identity.Pages.Account;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace CS4790GroupProject.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterEmployeeModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
            public RegisterEmployeeModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
           // IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _logger = logger;
                //_emailSender = emailSender;
                _roleManager = roleManager;
                _unitOfWork = unitOfWork;
            }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            public string Role { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            public string WeberNumber { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            [DataType(DataType.PhoneNumber)]
        
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
            public string PhoneNumber { get; set; }
        }

 public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/"); //null-coalescing assignment operator ??= assigns the value of right-hand operand to its left-hand operand only if the left-hand is nulll
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;

                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    ApplicationUser appUser = _unitOfWork.ApplicationUser.Get(a => a.Id == claim.Value);
                    bool isAdmin = await _userManager.IsInRoleAsync(appUser, "Admin");

                    if (isAdmin==true)
                    {
                        //expand identityuser with applicationuser properties
                        var user = new ApplicationUser
                        {
                            UserName = Input.Email,
                            Email = Input.Email,
                            FirstName = Input.FirstName,
                            LastName = Input.LastName,
                            PhoneNumber = Input.PhoneNumber,
                            WeberNumber = Input.WeberNumber
                         };

                        var result = await _userManager.CreateAsync(user, Input.Password);
                       

                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, Input.Role);
                            if (Input.Role=="Admin")
                            {
                                await _userManager.AddToRoleAsync(user, SD.EmployeeRole);
                            }
               
                            _logger.LogInformation("User created a new account with password.");
                            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            //var callbackUrl = Url.Page(
                            //    "/Account/ConfirmEmail",
                            //    pageHandler: null,
                            //    values: new { area = "Identity", userId = user.Id, code, returnUrl },
                            //    protocol: Request.Scheme);
                            //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            //   $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                            if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            {
                                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                            }
                            else
                            {
                                //redirect to employees page
                                return RedirectToPage("/Admin/Employees/Index");
                            }
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                    }
                }
                
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }

    }
}
