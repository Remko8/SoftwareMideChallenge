using RecruitmentChallange.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecruitmentChallange.ServiceInterfaces
{
    public interface IEmployeeService
    {
        public Task<List<Employee>> GetEmployeesAsync();
    }
}
