using HuntleyWeb.Application.Commands.enums;
using HuntleyWeb.Application.Data.Models.Bookings;
using MediatR;

namespace HuntleyWeb.Application.Commands.Bookings.Command
{
    public class BookingCommand : IRequest<BookingCommandResult>
    {
        public Booking Booking { get; set; }

        public BookingRate? Rate { get; set; }

        public CommandAction RequestedAction { get; set; }
    }
}
