using RecruitmentChallange.DTOs;
using RecruitmentChallange.Models;
using RecruitmentChallange.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentChallange.ServiceInterfaces
{
    public interface IReservationService
    {
        public Task<List<AdminReservationVM>> GetAllReservationsAdminAsync();
        public Task<List<EmployeeReservationVM>> GetAllReservationsEmployeeAsync();
        public bool IsMax7Days(ReservationDTO reservationDTO);
        public Task<bool> IsReservationAvaibleAsync(ReservationDTO reservationDTO);
        public Task<Reservation> AddReservationAsync(ReservationDTO reservationDTO, Employee currentUser);
        public Task RemoveReservationAsync(int reservationId);
        public Task<bool> ChangeReservationTimerAsync(ReservationChangeDTO reservation);
        public Task ChangeReservationAsync(ReservationChangeDTO reservation);
        public Task<bool> IsReservationValidAsync(ReservationChangeDTO reservation);
    }
}
