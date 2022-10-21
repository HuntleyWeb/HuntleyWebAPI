using HuntleyWeb.Application.Commands.BookingRates.Command.enums;
using HuntleyWeb.Application.Data.Models.Bookings;
using MediatR;

namespace HuntleyWeb.Application.Commands.BookingRates.Command
{
    public class BookingRateCommand : IRequest<RateCommandResult>
    {
        public BookingRate Rate { get; set; }  

        public CommandAction RequestedAction {  get; set; }
    }
}
