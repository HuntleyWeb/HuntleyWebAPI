using MediatR;

namespace HuntleyWeb.Application.Commands.BookingRates.Query
{
    public class BookingRateQuery : IRequest<SingleQueryResult>
    {
        public int RequestedYear { get; set; }

        public int RequestedWeekNumber { get; set; }
    }
}
