using System;
using Xunit;
using RecruitmentChallange.Services;
using System.Threading.Tasks;
using Moq;
using RecruitmentChallange.ServiceInterfaces;
using RecruitmentChallange.Models;
using RecruitmentChallange.Controllers;
using Microsoft.AspNetCore.Mvc;
using RecruitmentChallange.DTOs;

namespace Tests
{
    public class ServiceTests
    {
        [Fact]
        public async Task Throw_Add_Desk_Null_Exception()
        {
            var dbContext = new Mock<WebAppDBContext>();
            var deskService = new DeskService(dbContext.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await deskService.AddDeskAsync(null));
        }

        [Fact]
        public async Task Throw_Name_Taken_Exception()
        {
            var dbContext = new Mock<WebAppDBContext>();
            var locationService = new LocationService(dbContext.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await locationService.AddLocationAsync("terrace"));
        }
      
        [Fact]
        public void Register_Success()
        {

            Employee employee = new Employee
            {
                EmployeeId = 5,
                FirstName = "Anna",
                LastName = "Bond",
                Login = "emp2",
                Password = "emp321",
                Role = "Employee"
            };

            var iUserService = new Mock<IUserService>();

            var userController = new UserController(iUserService.Object);

            var newEmp = userController.Register(employee);

            Assert.IsType<CreatedResult>(newEmp);
        }
        [Fact]
        public async Task Add_Desk()
        {

            LocationDTO desk = new LocationDTO
            {
                locationName = "Zawodzie",
            };

            var iService = new Mock<IDeskService>();

            var controller = new DeskController(iService.Object);

            var newCntrl = await controller.AddDeskAsync(desk);

            Assert.IsType<StatusCodeResult>( newCntrl);
        }

    }
}
