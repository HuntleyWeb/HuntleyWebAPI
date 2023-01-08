using HuntleyWeb.Application.Data.Repos;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Commands.Bookings.Query
{
    public class BookingQueryHandler : IRequestHandler<BookingQuery, MultiQueryResult>
    {
        private readonly IBookingsRepository _bookingsRepository;

        public BookingQueryHandler(IBookingsRepository bookingsRepository)
        {
            _bookingsRepository = bookingsRepository;
        }

        public async Task<MultiQueryResult> Handle(BookingQuery request, CancellationToken cancellationToken)
        {
            // Fetch Bookings           
            var bookings = await _bookingsRepository.GetBookingsAsync(request.StartDate.Value, request.EndDate.Value);

            var result = new MultiQueryResult
            {
                Bookings = bookings,
            };

            return result;
        }
    }
}
