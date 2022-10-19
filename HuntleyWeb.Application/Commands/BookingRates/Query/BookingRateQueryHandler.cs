using HuntleyWeb.Application.Data.Repos;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Commands.BookingRates.Query
{
    public class BookingRateQueryHandler : IRequestHandler<BookingRateQuery, SingleQueryResult>
    {
        private readonly IBookingRateRepository _bookingRateRepository;

        public BookingRateQueryHandler(IBookingRateRepository bookingRateRepository)
        {
            _bookingRateRepository = bookingRateRepository;
        }

        public async Task<SingleQueryResult> Handle(BookingRateQuery request, CancellationToken cancellationToken)
        {
            // Fetch Booking Rate           
            var bookingRate = await _bookingRateRepository.GetBookingRateAsync(request.RequestedYear, request.RequestedWeekNumber);

            var result = new SingleQueryResult
            {
                Rate = bookingRate,
                RecordFound = (bookingRate != null) ? true : false
            };

            return result;
        }
    }
}
