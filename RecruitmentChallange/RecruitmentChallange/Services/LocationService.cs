using Microsoft.EntityFrameworkCore;
using RecruitmentChallange.Models;
using RecruitmentChallange.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentChallange.Services
{
    public class LocationService : BaseService, ILocationService
    {
        public LocationService(WebAppDBContext dbContext) : base(dbContext)
        {

        }
        
        // returns list of locations
        public async Task<List<Location>> GetLocationsAsync()
        {
            return await dbContext.Locations.ToListAsync();
        }
        // add a location with the given name
        public async Task AddLocationAsync(string locationName)
        { 
            if(locationName.Length>16)
                throw new Exception("Location name is too long");


            int? id = await dbContext.Locations.MaxAsync(l => (int?)l.LocationId);
            Location location = new Location
            {
                LocationId = (int)(id == null ? 0 : id+1),
                LocationName = locationName,
                DeskCount = 0
            };

            dbContext.Locations.Add(location);
            await dbContext.SaveChangesAsync();
        }

        // remove a location by the given name
        public async Task RemoveLocationAsync(string locationName)
        {
            Location location = await dbContext.Locations.FirstOrDefaultAsync(l => l.LocationName==locationName);

            dbContext.Locations.Remove(location);
            await dbContext.SaveChangesAsync();
        }

        // checks if the name for a new location is availbe
        public bool IsLocationNameAvaible(string locationName)
        {
            var isLocationNameAvaible = dbContext.Locations.Any(l => l.LocationName == locationName);
            return !isLocationNameAvaible;
        }

    }
}
