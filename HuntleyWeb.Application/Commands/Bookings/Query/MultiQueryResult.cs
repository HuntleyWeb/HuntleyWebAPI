using HuntleyWeb.Application.Data.Models.Bookings;
using System.Collections.Generic;

namespace HuntleyWeb.Application.Commands.Bookings.Query
{
    public class MultiQueryResult
    {
        public List<Booking> Bookings { get; set; }
    }
}
