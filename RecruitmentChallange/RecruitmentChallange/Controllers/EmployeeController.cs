using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentChallange.Models;
using RecruitmentChallange.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentChallange.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        // Returns list of employee
        [HttpGet]
        [Authorize(Roles="Administrator")]
        public async Task<JsonResult> GetEmployeeAsync() 
        {
            return new JsonResult(await employeeService.GetEmployeesAsync());
        }

    }
}
