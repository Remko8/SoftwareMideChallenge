using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RecruitmentChallange.Models
{
    public partial class Location
    {
        public Location()
        {
            Desks = new HashSet<Desk>();
        }

        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int DeskCount { get; set; }

        public virtual ICollection<Desk> Desks { get; set; }
    }
}
