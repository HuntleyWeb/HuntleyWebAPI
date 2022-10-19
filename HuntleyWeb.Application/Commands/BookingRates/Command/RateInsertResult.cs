using System;

namespace HuntleyWeb.Application.Commands.BookingRates.Command
{
    public class RateInsertResult
    {
        public bool Success { get; set; }

        public Guid RecordId { get; set; }
    }
}
