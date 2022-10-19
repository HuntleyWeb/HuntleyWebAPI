using HuntleyWeb.Application.Data.Models.Bookings;

namespace HuntleyWeb.Application.Commands.BookingRates.Query
{
    public class SingleQueryResult
    {
        public BookingRate Rate { get; set; }

        public bool RecordFound { get; set; }
    }
}
