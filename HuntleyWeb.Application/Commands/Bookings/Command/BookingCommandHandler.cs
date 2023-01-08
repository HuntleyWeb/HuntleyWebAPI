using HuntleyWeb.Application.Commands.enums;
using HuntleyWeb.Application.Data.Repos;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using HuntleyWeb.Application.Services.Bookings;

namespace HuntleyWeb.Application.Commands.Bookings.Command
{
    internal class BookingCommandHandler : IRequestHandler<BookingCommand, BookingCommandResult>
    {
        private readonly IBookingsRepository _bookingsRepository;
        private readonly ILogger<BookingCommandHandler> _logger;

        public BookingCommandHandler(
            IBookingsRepository bookingsRepository,
            ILogger<BookingCommandHandler> logger)
        {
            _bookingsRepository = bookingsRepository;
            _logger = logger;
        }

        public async Task<BookingCommandResult> Handle(BookingCommand request, CancellationToken cancellationToken)
        {
            var response = request.RequestedAction switch
            {
                CommandAction.Insert => await ProcessBookingInsert(request),
                //CommandAction.Update => await ProcessRateUpdate(request),
                //CommandAction.Delete => await ProcessRateDelete(request),
                _ => throw new Exception("No Command Action Set in Request")
            };

            return response;
        }

        private async Task<BookingCommandResult> ProcessBookingInsert(BookingCommand request)
        {
            // Check for existing Rate with Same WeekNumber and Year
            var existingBooking = await _bookingsRepository.GetBookingAsync(request.Booking.StartDate, request.Booking.EndDate);

            if (existingBooking != null)
            {
                return new BookingCommandResult
                {
                    Success = false,
                    RecordsAffected = 0,
                    CommandResult = CommandActionResult.Conflict,
                    RecordId = existingBooking.Id,
                    Information = $"Conficting booking already exists starting:{existingBooking.StartDate} and ending:{existingBooking.EndDate}"
                };
            }

            var (duration, breakType) = BookingsHelper.GetBookingType(request.Booking.StartDate, request.Booking.EndDate);

            // Finalize Request Data                       
            request.Booking.Id = Guid.NewGuid();
            request.Booking.Year = request.Booking.StartDate.Year;
            request.Booking.BookingDate = DateTime.UtcNow;
            request.Booking.Created = DateTime.UtcNow;
            request.Booking.Days = duration;
            request.Booking.Confirmed = false;
            request.Booking.BookingType = breakType.ToString();
            
            if (request.Rate != null)
            {
                if (!request.Rate.AvailableForRental)
                {
                    throw new Exception("Cottage is no longer available during this period");
                }

                request.Booking.Rate = breakType switch
                {
                    BreakType.SevenDay => request.Rate.SevenDayRate,
                    BreakType.Weekend => request.Rate.WeekendRate,
                    BreakType.MidWeek => request.Rate.MidWeekRate,
                    _ => throw new Exception("No matching Booking Rate Set!")
                };
            }

            request.Booking.LastModified = null;

            var bookingId = await _bookingsRepository.CreateNewBooking(request.Booking);

            if (bookingId != Guid.Empty)
            {
                return new BookingCommandResult
                {
                    Success = true,
                    RecordsAffected = 1,
                    CommandResult = CommandActionResult.Created,
                    RecordId = bookingId
                };
            }

            return new BookingCommandResult
            {
                Success = false,
                RecordId = bookingId,
                RecordsAffected = 0,
                CommandResult = CommandActionResult.Failure
            };
        }
    }
}
