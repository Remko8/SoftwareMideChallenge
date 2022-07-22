using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentChallange.DTOs;
using RecruitmentChallange.Models;
using RecruitmentChallange.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentChallange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeskController : ControllerBase
    {
        private IDeskService deskService;

        public DeskController(IDeskService deskService)
        {
            this.deskService = deskService;
        }

        // Returns list of desks
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDesksAsync()
        {
            var desks = await deskService.GetDesksAsync();
            return Ok(desks);
        }


        /// <Task 2> Part(1/2)
        /// Manage desk in locations (add/remove if no reservation/make unavailable)
        /// </Task>
        /// <summary> Controller adds new desk to given location.</summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddDeskAsync([FromBody] LocationDTO locationDTO)
        {
            await deskService.AddDeskAsync(locationDTO.locationName);

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <Task 2> Part(2/2)
        /// Manage desk in locations (add/remove if no reservation/make unavailable)
        /// </Task>
        /// <summary> Controller remove desk by id. Returns 406 if given desk have reservations </summary>
        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RemoveDeskAsync([FromBody] int deskId)
        {
            Desk desk = deskService.GetDesksAsync().Result.FirstOrDefault(d => d.DeskId == deskId);

            if (desk == null)
                return StatusCode(406, new { status = 406, message = "desk does not exist" });

            if (desk.ReservationCount > 0)
                return StatusCode(406, new { status = 406, message = "The desk cannot be removed because there are active reservations." });

            await deskService.RemoveDeskAsync(deskId);

            return Ok();
        }

        /// <Task 4> 
        /// Filter desks based on location
        /// </Task>
        [HttpGet("Filter")]
        [Authorize]
        public async Task<IActionResult> FilterDesksAsync([FromBody] LocationDTO locationDTO)
        {
            var filteredDesks = await deskService.FilterDesksAsync(locationDTO.locationName);

            return Ok(filteredDesks);
        }
    }
}
