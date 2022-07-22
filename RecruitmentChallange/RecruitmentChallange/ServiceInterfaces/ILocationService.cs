using RecruitmentChallange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentChallange.ServiceInterfaces
{
    public interface ILocationService
    {
        public Task<List<Location>> GetLocationsAsync();
        public Task AddLocationAsync(string locationName);
        public Task RemoveLocationAsync(string locationName);
        public bool IsLocationNameAvaible(string locationName);
    }
}
