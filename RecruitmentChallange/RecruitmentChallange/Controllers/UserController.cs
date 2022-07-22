using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RecruitmentChallange.DTOs;
using RecruitmentChallange.Models;
using RecruitmentChallange.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecruitmentChallange.Controllers
{
    // user controller created for authorization and authentication purposes
    // users: administrator: {Login: admin Password: admin123}
    //        employee:      {Login: emp Password: emp123}

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService userService;

        public UserController(IUserService login)
        {
            this.userService = login;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
             var user = this.userService.Authentization(loginDTO);

            if (user == null)
                return Unauthorized(new { message = "Username or password is incorrect.", status = 401 });

            TokenDTO token = this.userService.GenerateToken(user);

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(Employee employee)
        {
            if (employee == null)
            {
                employee = new Employee
                {
                    FirstName = "Anna",
                    LastName = "Bond",
                    Login = "emp2",
                    Password = "emp321",
                    Role = "Employee"
                };
            }

            bool credentialsTaken = userService.AreUserCredentialsTaken(employee);
            if(credentialsTaken == true)
                return StatusCode(406, new { status = 406, message = "credentials already taken" });

            userService.Register(employee);

            return Created("/login", employee);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetUsers()
        {
            var users = userService.GetUsers().ToList();
            return Ok(users);
        }

        [HttpGet("Me")]
        public IActionResult CheckUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var currentUser = userService.GetCurrentUser(identity);

            return Ok($"Hello {currentUser.FirstName}, you are {currentUser.Role}");
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return Ok();
        }


    }
}
