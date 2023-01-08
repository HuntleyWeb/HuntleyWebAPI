using HuntleyWeb.Application.Data.Models.Bookings;
using System;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Data.Repos
{
    public interface IBookingsRepository
    {
        Task<Guid> CreateNewBooking(Booking booking);
        Task<bool> DeleteBooking(Booking booking);
        Task<Booking> GetBookingAsync(Booking booking);
        Task<Booking> GetBookingAsync(DateTime startDate, DateTime endDate);
    }
}