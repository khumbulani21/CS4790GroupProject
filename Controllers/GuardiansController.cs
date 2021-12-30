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
    public class GuardiansController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public GuardiansController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get()
        {

            var test = _unitOfWork.Guardian.List(null, null, "FamilyGroups.Child").ToList();
            return Json(new { data = _unitOfWork.Guardian.List(null, includes: "FamilyGroups.Child") });
        }
    }
}
