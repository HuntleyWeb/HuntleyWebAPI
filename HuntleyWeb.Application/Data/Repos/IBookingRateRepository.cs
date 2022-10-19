using HuntleyWeb.Application.Data.Models.Bookings;
using System;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Data.Repos
{
    public interface IBookingRateRepository
    {
        Task<Guid> CreateNewBookingRate(BookingRate rate);

        Task<BookingRate> GetBookingRateAsync(int year, int weekNumber);

        Task<Guid> UpsertBookingRate(BookingRate rate);
    }
}