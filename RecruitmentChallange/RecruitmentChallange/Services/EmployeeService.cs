using Microsoft.EntityFrameworkCore;
using RecruitmentChallange.Models;
using RecruitmentChallange.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentChallange.Services
{
    public class EmployeeService : BaseService, IEmployeeService
    {

        public EmployeeService(WebAppDBContext dbContext) : base(dbContext)
        {

        }

        // returns list of employees
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            List<Employee> employees = await dbContext.Employees.ToListAsync();

            return employees;
        }
    }
}
