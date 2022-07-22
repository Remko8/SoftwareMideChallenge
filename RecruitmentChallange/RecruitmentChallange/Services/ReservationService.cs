using Microsoft.EntityFrameworkCore;
using RecruitmentChallange.DTOs;
using RecruitmentChallange.Models;
using RecruitmentChallange.ServiceInterfaces;
using RecruitmentChallange.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecruitmentChallange.Services
{
    public class ReservationService : BaseService, IReservationService
    {
        public ReservationService(WebAppDBContext dbContext) : base(dbContext)
        {

        }

        /// <Task 3> 
        /// Determine which desks are available to book or unavailable.
        /// </Task>
        /// <summary> Service determines if a reservation is avaible </summary>
        public async Task<bool> IsReservationAvaibleAsync(ReservationDTO reservationDTO)
        {
            var ReservationsDeskID = await Task.Run(() => dbContext.Reservations.Where(d => d.DeskId == reservationDTO.DeskId).ToListAsync());
            if (ReservationsDeskID.Count() == 0)
                return true;

            var reservation = await Task.Run(() => ReservationsDeskID.All(r =>
                (r.ReservationStart > reservationDTO.ReservationStart && r.ReservationStart > reservationDTO.ReservationEnd)
                || (r.ReservationEnd < reservationDTO.ReservationStart && r.ReservationEnd < reservationDTO.ReservationEnd)));

            return reservation;
        }

        // returns data avaible to administrator
        public async Task<List<AdminReservationVM>> GetAllReservationsAdminAsync()
        {
            var adminReservationsVM = new List<AdminReservationVM>();

            adminReservationsVM = await AddReservationDataAsync(adminReservationsVM);
            adminReservationsVM = await AddLocationDataAsync(adminReservationsVM);
            adminReservationsVM = await AddEmployeeDataAsync(adminReservationsVM);

            return adminReservationsVM;
        }
        // returns data avaible to employee
        public async Task<List<EmployeeReservationVM>> GetAllReservationsEmployeeAsync()
        {
            var employeeReservationsVM = new List<EmployeeReservationVM>();

            employeeReservationsVM = await AddReservationDataAsync(employeeReservationsVM);
            employeeReservationsVM = await AddLocationDataAsync(employeeReservationsVM);
            
            return employeeReservationsVM.Cast<EmployeeReservationVM>().ToList();
        }
        // returns reservation data
        private async Task<List<T>> AddReservationDataAsync<T>(List<T> employeeReservationsVM) where T : EmployeeReservationVM, new()
        {
            var reservations = await dbContext.Reservations.ToListAsync();

            foreach (var reservation in reservations)
            {
                employeeReservationsVM.Add(new T
                {
                    ReservationId = reservation.ReservationId,
                    DeskId = reservation.DeskId,
                    ReservationStart = reservation.ReservationStart,
                    ReservationEnd = reservation.ReservationEnd
                });
            }

            return employeeReservationsVM;
        }
        // returns location data
        private async Task<List<T>> AddLocationDataAsync<T>(List<T> employeeReservationsVM) where T : EmployeeReservationVM, new()
        {
            var desks = await dbContext.Desks.ToListAsync();
            var locations = await dbContext.Locations.ToListAsync();

            foreach (var desk in desks)
            {
                employeeReservationsVM.Where(e => e.DeskId == desk.DeskId).ToList()
                    .Select(e => (
                        e.LocationName = dbContext.Locations.FirstAsync(l => l.LocationId == desk.LocationId).Result.LocationName,
                        e.LocationId = dbContext.Locations.FirstAsync(l => l.LocationId == desk.LocationId).Result.LocationId
                    )).ToList();
            }

            return employeeReservationsVM;
        }

        // returns employee data
        private async Task<List<AdminReservationVM>> AddEmployeeDataAsync(List<AdminReservationVM> adminReservationsVM)
        {
            var employees = await dbContext.Employees.ToListAsync();
            foreach (var employee in employees)
            {
                adminReservationsVM.Where(e => e.EmployeeId == employee.EmployeeId).ToList()
                    .Select(e => (
                        e.EmployeeId = employee.EmployeeId,
                        e.FirstName = employee.FirstName,
                        e.LastName = employee.LastName
                    )).ToList();
            }

            return adminReservationsVM;
        }

        // checks if a timeframe of a reservation is smaller than week
        public bool IsMax7Days(ReservationDTO reservationDTO)
        {
            var timeDiff = reservationDTO.ReservationEnd - reservationDTO.ReservationStart;
            if (timeDiff.Days <= 7)            
                return true;
            
            return false;
        }

        // adds a new reservation
        public async Task<Reservation> AddReservationAsync(ReservationDTO reservationDTO, Employee currentUser)
        {
            int? id = dbContext.Reservations.MaxAsync(d => (int?)d.ReservationId).Result;

            Reservation reservation = new Reservation
            {
                ReservationId = (int)(id == null ? 0 : id + 1),
                DeskId = reservationDTO.DeskId,
                EmployeeId = currentUser.EmployeeId, 
                ReservationStart = reservationDTO.ReservationStart,
                ReservationEnd = reservationDTO.ReservationEnd
            };

            var desk = await dbContext.Desks.FirstAsync(d=>d.DeskId == reservation.DeskId);

            if (reservation == null || desk == null)
                throw new ArgumentNullException("Null error at adding reservation");

            desk.ReservationCount += 1;
            dbContext.Reservations.Add(reservation);
            await dbContext.SaveChangesAsync();
            return reservation;
        }
        // remove reservation by Id
        public async Task RemoveReservationAsync(int reservationId)
        {
            var reservation = dbContext.Reservations.FirstOrDefaultAsync(r => r.ReservationId == reservationId).Result;
            var desk = await dbContext.Desks.FirstAsync(d => d.DeskId == reservation.DeskId);

            if (reservation == null || desk == null)
                throw new ArgumentNullException("Null error at removing reservation");

            desk.ReservationCount -= 1;
            dbContext.Reservations.Remove(reservation);
            await dbContext.SaveChangesAsync();
        }

        // checks if the reservation change is greater than 24h
        public async Task<bool> ChangeReservationTimerAsync(ReservationChangeDTO reservationChangeDTO)
        {
            Reservation reservation = await dbContext.Reservations.FirstAsync(r => r.ReservationId == reservationChangeDTO.ReservationId);
            var timeDiff = (reservation.ReservationStart - DateTime.Now).Hours;

            if (timeDiff < 24)
                return true;
            return false;
        }
        // changes a desk in the given reservation
        public async Task ChangeReservationAsync(ReservationChangeDTO reservationChangeDTO)
        {            
            Reservation reservation = await dbContext.Reservations.FirstAsync(r => r.ReservationId == reservationChangeDTO.ReservationId);

            var _reservation = dbContext.Reservations.FirstOrDefaultAsync(r => r.ReservationId == reservation.ReservationId).Result;



            var newDesk = await dbContext.Desks.FirstAsync(d => d.DeskId == reservationChangeDTO.DeskId);
            var oldDesk = await dbContext.Desks.FirstAsync(d=>d.DeskId == reservation.DeskId);

            oldDesk.ReservationCount -= 1;
            newDesk.ReservationCount += 1;
            _reservation.DeskId = reservationChangeDTO.DeskId;
            await dbContext.SaveChangesAsync();
        }
        // map classes
        private ReservationDTO ReservationToDTO(Reservation reservation, ReservationChangeDTO reservationChangeDTO)
        {
            ReservationDTO reservationDTO = new ReservationDTO
            {
                DeskId = reservationChangeDTO.DeskId,
                ReservationStart = reservation.ReservationStart,
                ReservationEnd = reservation.ReservationEnd
            };
            return reservationDTO;
        }
        // checks if data for reservation is valid
        public async Task<bool> IsReservationValidAsync(ReservationChangeDTO reservationChangeDTO)
        {
            Reservation reservation = await dbContext.Reservations.FirstAsync(r => r.ReservationId == reservationChangeDTO.ReservationId);
            ReservationDTO reservationDTO = await Task.Run(() => ReservationToDTO(reservation, reservationChangeDTO));
            var isReservationValid = await IsReservationAvaibleAsync(reservationDTO);

            return isReservationValid;
        }
    }
}
