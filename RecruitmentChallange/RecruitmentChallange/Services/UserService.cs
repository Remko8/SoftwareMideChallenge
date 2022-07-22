using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecruitmentChallange.DTOs;
using RecruitmentChallange.Models;
using RecruitmentChallange.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentChallange.Services
{
    public class UserService : BaseService, IUserService
    {
        private IConfiguration configuration;
        public UserService(WebAppDBContext dbContext, IConfiguration configuration) : base(dbContext)
        {
            this.configuration = configuration;
        }
        public TokenDTO GenerateToken(Employee user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.EmployeeId.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            TokenDTO _token = new TokenDTO { token = new JwtSecurityTokenHandler().WriteToken(token) };

            return _token;
        }
        public Employee Authentization(LoginDTO loginDTO)
        {
            var currentUser = dbContext.Employees.FirstOrDefault(e => e.Login == loginDTO.Login && e.Password == loginDTO.Password);
            if (currentUser != null)
                return currentUser;
            return null;
        }

        public Employee GetCurrentUser(ClaimsIdentity _identity)
        {
            var identity = _identity;

            if (identity == null)
                return null;

            var userClaims = identity.Claims;
            return new Employee
            {
                EmployeeId = Int32.Parse(userClaims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value),
                Login = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value,
                FirstName = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.GivenName)?.Value,
                LastName = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Surname)?.Value,
                Role = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Role)?.Value
            };
        }

        public List<Employee> GetUsers()
        {
            var users = dbContext.Employees.ToList();

            return users;
        }

        public void Register(Employee employee)
        {
            UserValidation(employee);

            var id = dbContext.Employees.Max(e => e.EmployeeId);

            employee.EmployeeId = id+1;
            dbContext.Employees.Add(employee);
            dbContext.SaveChanges();
        }

        public void UserValidation(Employee employee)
        {
            var maxLenght = 16;
            if (employee.FirstName.Length > maxLenght || employee.LastName.Length > maxLenght || employee.Login.Length > maxLenght || employee.Password.Length > maxLenght)
                throw new Exception("Registration text is too long");
        }

        public bool AreUserCredentialsTaken(Employee employee)
        {
            bool credentialsTaken = GetUsers().Any(u => u.Login == employee.Login || u.Password == employee.Password);
            return credentialsTaken;
        }
    }
}
