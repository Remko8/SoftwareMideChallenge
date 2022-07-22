using RecruitmentChallange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentChallange.ServiceInterfaces
{
    public interface IDeskService
    {
        public Task<List<Desk>> GetDesksAsync();
        public Task AddDeskAsync(string locationName);
        public Task RemoveDeskAsync(int deskId);

        public Task<List<Desk>> FilterDesksAsync(string locationName);

    }
}
