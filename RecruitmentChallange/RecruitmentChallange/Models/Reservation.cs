using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RecruitmentChallange.Models
{
    public partial class Reservation
    {
        public int ReservationId { get; set; }
        public int DeskId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ReservationStart { get; set; }
        public DateTime ReservationEnd { get; set; }

        public virtual Desk Desk { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
