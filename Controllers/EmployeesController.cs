using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.ViewModel;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CS4790GroupProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : Controller
    {
         
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;


        public EmployeesController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

 


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string loggedInUser=null;
            if (claim!=null)
            {
                 loggedInUser = claim.Value;

            }
         

            List<EmployeeVM> employees = new List<EmployeeVM>();
          
            var employeeList = await _userManager.GetUsersInRoleAsync(SD.EmployeeRole);
            var admins = await _userManager.GetUsersInRoleAsync(SD.AdminRole);
            
            foreach (var user in admins)
            {
                List<string> isLocked = new List<string>();
                var lockedStatus = await _userManager.IsLockedOutAsync(user);
                List<string> role = new List<string>();
                //check if they also have employee role
                //var role = _userManager.IsInRoleAsync(user,SD.EmployeeRole);
                if (loggedInUser==user.Id)
                {
                    continue;
                }
                isLocked.Add(user.Id);
                isLocked.Add(lockedStatus.ToString());
                role.Add(user.Id);
                role.Add("Admin");
                if (employeeList.Contains(user))
                {
                    employees.Add(new EmployeeVM { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName, Role = role, Locked = isLocked });
                }
                
            }
            //admin shouldn't see his account listed
            //should be able to see other admins
            foreach (var user in employeeList)
            {
                List<string> isLocked = new List<string>();
                List<string> role = new List<string>();
                var lockedStatus = await _userManager.IsLockedOutAsync(user);
                //var role= await _userManager.GetRolesAsync(user);
                if (loggedInUser == user.Id)
                {
                    continue;
                }
                if (admins.Contains(user))
                {
                    continue;
                }
                isLocked.Add(user.Id);
                isLocked.Add(lockedStatus.ToString());
                role.Add(user.Id);
                role.Add("Employee");
                employees.Add(new EmployeeVM { Id=user.Id,FirstName=user.FirstName, LastName=user.LastName,UserName = user.UserName, Role =role, Locked= isLocked });
            }
            return Json(new { data = employees });

        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser appUser = _unitOfWork.ApplicationUser.Get(c => c.Id == id);
            var role = await _userManager.GetRolesAsync(appUser);
       
            if (appUser == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            _unitOfWork.ApplicationUser.Delete(appUser);
             _unitOfWork.Commit();

            return Json(new { success = true, message = appUser.FullName+ " has been deleted " });
        }


       
        [Route("[action]/{id}")]
        public async Task<IActionResult>  PostLockUnlock(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            string message = "";

            if (user == null)
            {
                return Json(new { success = false, message = "Error changing role" });
            }
            if (user.LockoutEnd == null)
            {
                message = user.FullName + " has been locked  ";
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }
            else if (user.LockoutEnd > DateTime.Now)
            {
                message = user.FullName + " has been unlocked  ";
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                message = user.FullName + " has been locked  ";
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }
            _unitOfWork.ApplicationUser.Update(user);
            await _unitOfWork.CommitAsync();
            return Json(new { success = true, message = message});
        }

      
        [Route("[action]/{id}")]
        public async Task<IActionResult> PostAdminEmployee(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error changing role" });
            }
            var adminRole=await _userManager.IsInRoleAsync(user,SD.AdminRole);
            var empRole = await _userManager.IsInRoleAsync(user, SD.EmployeeRole);
             
            if (empRole == true&& adminRole==true)
            {
                //user is an admin remove admin role
                await _userManager.RemoveFromRoleAsync(user, SD.AdminRole);
            }
            else if (empRole == true &&  adminRole == false)
            {
                //make admin
                await _userManager.AddToRoleAsync(user,SD.AdminRole);
                
            }
            else
            {
               
            }
            return Json(new { success = true, message = "Role changed successful " });
        }
    }

}
