using RecruitmentChallange.DTOs;
using RecruitmentChallange.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace RecruitmentChallange.ServiceInterfaces
{
    public interface IUserService
    {
        public TokenDTO GenerateToken(Employee user);
        public Employee Authentization(LoginDTO loginDTO);
        public Employee GetCurrentUser(ClaimsIdentity _identity);
        public List<Employee> GetUsers();
        public void Register(Employee employee);
        public bool AreUserCredentialsTaken(Employee employee);
    }
}
