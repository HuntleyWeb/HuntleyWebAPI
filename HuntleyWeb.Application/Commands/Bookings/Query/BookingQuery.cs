using MediatR;
using System;

namespace HuntleyWeb.Application.Commands.Bookings.Query
{
    public class BookingQuery : IRequest<MultiQueryResult>
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set;}
    }
}
