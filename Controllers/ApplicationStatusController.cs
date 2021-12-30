using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS4790GroupProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationStatusController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApplicationStatusController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        [HttpGet]
        public IActionResult Get()
        {
         var applications = from c in _unitOfWork.Child.List()
                         join a in _unitOfWork.Application.List()
                         on c.ChildID equals a.ChildID
                         join p in _unitOfWork.Program.List()
                         on a.ProgramID equals p.ProgramID
                         into Group
                         from p in Group.DefaultIfEmpty()
                         select new
                         {
                             ChildId=c.ChildID,
                             ChildFirst = c.ChildFirst,
                             ChildLast = c.ChildLast,
                             ProgramID = a.ProgramID,
                             ProgramName=p.ProgramName,
                             ApplicationId= a.ApplicationID,
                             ApplicationStatus= a.ApplicationStatus  ,
                             ModifiedDate=a.ModifiedDate.Date.ToShortDateString(),
                             AppIdStatus = new string[] { a.ApplicationID.ToString(), a.ApplicationStatus }

                         };
          
             return Json(new { data = applications });
        }

            [Route("[action]/{id}")]
        public IActionResult PostApprove(int id)
        {
            Application application = _unitOfWork.Application.Get(u => u.ApplicationID == id);
            if (application == null)
            {
                return Json(new { success = false, message = "Error changing status" });
            }
            else
            {
                application.ApplicationStatus = "Approved";
                _unitOfWork.Application.Update(application);
                _unitOfWork.Commit();
            }
         return Json(new { success = true, message = "Status changed successfully " });
        }


        [Route("[action]/{id}")]
        public IActionResult PostDecline(int id)
        {
            Application application = _unitOfWork.Application.Get(u => u.ApplicationID == id);
            if (application == null)
            {
                return Json(new { success = false, message = "Error changing status" });
            }
            else
            {
                application.ApplicationStatus = "Denied";
                _unitOfWork.Application.Update(application);
                _unitOfWork.Commit();
            }
            return Json(new { success = true, message = "Status changed successfully " });
        }

        [Route("[action]/{id}")]
        public IActionResult PostWaitlist(int id)
        {
            Application application = _unitOfWork.Application.Get(u => u.ApplicationID == id);
            if (application == null)
            {
                return Json(new { success = false, message = "Error changing status" });
            }
            else
            {
                application.ApplicationStatus = "Waitlist";
                _unitOfWork.Application.Update(application);
                _unitOfWork.Commit();
            }
            return Json(new { success = true, message = "Status changed successfully " });
        }
    }
}
