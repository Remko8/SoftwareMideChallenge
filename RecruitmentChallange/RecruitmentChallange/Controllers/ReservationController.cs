using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private IReservationService reservationService;
        private IUserService userService;

        public ReservationController(IReservationService reservationService, IUserService userService)
        {
            this.reservationService = reservationService;
            this.userService = userService;
        }

        /// <Task 8> Part(1/2)
        /// Administrators can see who reserves a desk in location, where Employees can see only that specific desk is unavailable.
        /// </Task>
        /// <Summary> 
        /// Reservations with data avaible for administrator.
        /// Returns List of reservations with employee and location information
        /// </Summary>
        [HttpGet("Admin")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllreservationsAdminAsync()
        {
            var AllReservations = await reservationService.GetAllReservationsAdminAsync();
            return Ok(AllReservations);
        }



        /// <Task 8> Part(2/2)
        /// Administrators can see who reserves a desk in location, where Employees can see only that specific desk is unavailable.
        /// </Task>
        /// <Summary> 
        /// Reservations with data avaible for employee.
        /// Returns List of reservations without employees data
        /// </Summary>
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetAllReservationsEmployeeAsync()
        {
            var AllReservations = await reservationService.GetAllReservationsEmployeeAsync();
            return Ok(AllReservations);
        }

        /// <Task 5 & 6> 
        /// 5) Book a desk for the day.
        /// 6) Allow reserving a desk for multiple days but now more than a week.
        /// </Task>
        /// <Summary> 
        /// Adds reservation for given timeframe.
        /// Returns response 406 if time exceeds 7 days or booking time overlaps with another reservation
        /// </Summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReservationAsync([FromBody] ReservationDTO reservationDTO)
        {
            var isValidTime = await Task.Run(()=>reservationService.IsMax7Days(reservationDTO));
            var isAvaible = await reservationService.IsReservationAvaibleAsync(reservationDTO);

            if (isValidTime == false)
                return StatusCode(406, new { status = 406, message = "Reservation time exceeds 7 days." });

            if (isAvaible == false)
                return StatusCode(406, new { status = 406, message = "Cannot create reservation" });

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var currentUser = userService.GetCurrentUser(identity);
            var reservation = await reservationService.AddReservationAsync(reservationDTO, currentUser);

            //return Created($"reservation/{reservation.ReservationId}", reservation);
            return StatusCode(StatusCodes.Status201Created, reservation);
        }

        // Remove reservation
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveReservationAsync([FromBody] int reservationId)
        {
            await reservationService.RemoveReservationAsync(reservationId);

            return Ok();
        }

        /// <Task7> 
        /// Allow to change desk, but not later than the 24h before reservation.
        /// </Task>
        /// <Summary> 
        /// Change desk in given reservation
        /// Returns response 406 if time doesn't exceeds 24h
        /// </Summary>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ChangeReservationAsync([FromBody] ReservationChangeDTO reservation)
        {
            var isReservationValid = await reservationService.IsReservationValidAsync(reservation);
            if (isReservationValid == false)
            {
                return StatusCode(406, new { status = 406, message = "The Desk to which the reservation tried move, has alredy been booked in this timeframe" });
            }
            var isTimeLimitExceeded = await  reservationService.ChangeReservationTimerAsync(reservation);
            if (isTimeLimitExceeded == false)
            {
                return StatusCode(406, new { status = 406, message = "Time limit 24h before the start of the reservation has been exceeded" });
            }

            await reservationService.ChangeReservationAsync(reservation);

            return Ok();
        }
    }
}
