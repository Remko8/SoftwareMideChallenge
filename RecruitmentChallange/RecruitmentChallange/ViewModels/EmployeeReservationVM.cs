using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentChallange.ViewModels
{
    public class EmployeeReservationVM
    {
        public int ReservationId { get; set; }
        public int DeskId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public DateTime ReservationStart { get; set; }
        public DateTime ReservationEnd { get; set; }
    }
}
