using ApplicationCore.Interfaces;
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
    public class ProgramsController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;



        public ProgramsController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;




        [HttpGet]
        public IActionResult Get()
        {

            return Json(new { data = _unitOfWork.Program.List() });

        }
    }
}
