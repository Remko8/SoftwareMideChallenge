using Microsoft.EntityFrameworkCore;
using RecruitmentChallange.Models;
using RecruitmentChallange.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentChallange.Services
{
    public class DeskService : BaseService, IDeskService
    {
        public DeskService(WebAppDBContext dbContext) : base(dbContext)
        {

        }

        // returns list of desks
        public async Task<List<Desk>> GetDesksAsync()
        {
            return await dbContext.Desks.ToListAsync();
        }

        // adds a new desk to the given location
        public async Task AddDeskAsync(string locationName)
        {
            int? id = await dbContext.Desks.MaxAsync(d => (int?)d.DeskId);
            Location location = await dbContext.Locations.FirstOrDefaultAsync(l => l.LocationName == locationName);
            if (locationName == null || location == null)
                throw new ArgumentNullException("Location does not exist");

            Desk desk = new Desk
            {
                DeskId = (int)(id == null ? 0 : id + 1),
                LocationId = location.LocationId,
                ReservationCount = 0,
                Location = location                
            };

            location.DeskCount += 1;
            dbContext.Desks.Add(desk);
            await dbContext.SaveChangesAsync();
        }

        // remove a desk by id
        public async Task RemoveDeskAsync(int deskId)
        {
            Desk desk = await dbContext.Desks.FirstOrDefaultAsync(d => d.DeskId == deskId);
            Location location = await dbContext.Locations.FirstOrDefaultAsync(l => l.LocationId == desk.LocationId);

            if (desk == null)
                throw new Exception("Desk not found");

            location.DeskCount -= 1;
            dbContext.Desks.Remove(desk);
            await dbContext.SaveChangesAsync();
        }

        // filter desks by a location name
        public async Task<List<Desk>> FilterDesksAsync(string locationName)
        {
            List<Desk> filteredDesks = await dbContext.Desks.Where(d => d.Location.LocationName == locationName).ToListAsync();

            return filteredDesks;
        }
    }
}
