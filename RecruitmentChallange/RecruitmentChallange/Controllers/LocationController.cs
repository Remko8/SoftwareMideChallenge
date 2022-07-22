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
    public class LocationController : ControllerBase
    {
        private ILocationService locationService;

        public LocationController(ILocationService locationService)
        {
            this.locationService = locationService;
        }

        // Returns list of locations
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLocationsAsync()
        {
            var locations = await locationService.GetLocationsAsync();
            return Ok(locations);
        }

        /// <Task 1> Part(1/2)
        /// Manage locations(add/remove, can't remove if desk exists in location)
        /// </Task>
        /// <summary> Controller adds new location. </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        //[AllowAnonymous]
        public async Task<IActionResult> AddLocationAsync([FromBody] LocationDTO locationDTO)
        {
            var isLocationNameAvaible = await Task.Run(() => locationService.IsLocationNameAvaible(locationDTO.locationName));
            if (isLocationNameAvaible == false)
                return StatusCode(406, new { status = 406, message = "location name is already taken." });

            await locationService.AddLocationAsync(locationDTO.locationName);

            return StatusCode(StatusCodes.Status201Created);
        }


        /// <Task 1> Part(2/2)
        /// Manage locations(add/remove, can't remove if desk exists in location)
        /// </Task>
        /// <summary> Controller remove location. </summary>
        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RemoveLocationAsync([FromBody] LocationDTO locationDTO)
        {
            Location location = locationService.GetLocationsAsync().Result.FirstOrDefault(l => l.LocationName == locationDTO.locationName);
            if (location == null)
                return StatusCode(406, new { status = 406, message = "Location does not exist" });
        
            if (location.DeskCount > 0)
                return StatusCode(406, new { status = 406, message = "The location cannot be removed because there are desks there." });

            await locationService.RemoveLocationAsync(locationDTO.locationName);

            return Ok();
        }
    }
}
