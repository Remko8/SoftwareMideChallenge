using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RecruitmentChallange.Models
{
    public partial class Desk
    {
        public Desk()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int DeskId { get; set; }
        public int LocationId { get; set; }
        public int ReservationCount { get; set; }

        public virtual Location Location { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
