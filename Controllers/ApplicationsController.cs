using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using CS4790GroupProject.ViewModel;
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
    public class ApplicationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        ApplicationDetailsVM ApplicationVM;
        public ApplicationsController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Application> applications=_unitOfWork.Application.List();
            IEnumerable<Guardian> guardians = _unitOfWork.Guardian.List();
            IEnumerable<Child> children = _unitOfWork.Child.List();
            ApplicationVM = new ApplicationDetailsVM
            {
                Applications=applications,
                Guardians=guardians,
                Children=children

            };


            return Json(new { data = _unitOfWork.Application.List() });

        }

    }
}
