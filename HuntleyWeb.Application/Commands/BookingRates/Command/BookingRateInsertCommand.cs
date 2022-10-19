using HuntleyWeb.Application.Data.Models.Bookings;
using MediatR;

namespace HuntleyWeb.Application.Commands.BookingRates.Command
{
    public class BookingRateInsertCommand : IRequest<RateInsertResult>
    {
        public BookingRate NewBookingRate { get; set; }
    }
}
