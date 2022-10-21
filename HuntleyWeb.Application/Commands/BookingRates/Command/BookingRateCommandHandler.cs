using Azure.Core;
using HuntleyWeb.Application.Data.Models.Bookings;
using HuntleyWeb.Application.Data.Repos;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Commands.BookingRates.Command
{
    public class BookingRateCommandHandler : IRequestHandler<BookingRateCommand, RateCommandResult>
    {
        private readonly IBookingRateRepository _bookingRateRepository;
        private readonly ILogger<BookingRateCommandHandler> _logger;

        public BookingRateCommandHandler(
            IBookingRateRepository bookingRateRepository, 
            ILogger<BookingRateCommandHandler> logger)
        {
            _bookingRateRepository = bookingRateRepository;
            _logger = logger;
        }

        public async Task<RateCommandResult> Handle(BookingRateCommand request, CancellationToken cancellationToken)
        {            
            var response  = request.RequestedAction switch
            {
                enums.CommandAction.Insert => await ProcessRateInsert(request),
                enums.CommandAction.Update => await ProcessRateUpdate(request),
                enums.CommandAction.Delete => await ProcessRateDelete(request),
                _ => throw new Exception("No Command Action Set in Request")
            };
            
            return response;
        }

        private async Task<RateCommandResult> ProcessRateInsert(BookingRateCommand request)
        {
            // Check for existing Rate with Same WeekNumber and Year
            var existingRate = await _bookingRateRepository.GetBookingRateAsync(request.Rate.Year, request.Rate.WeekNumber);

            if (existingRate != null)
            {
                return new RateCommandResult
                {
                    Success = false,
                    RecordsAffected = 0,
                    CommandResult = enums.CommandActionResult.Conflict,
                    RecordId = existingRate.Id,
                    Information = $"Booking Rate already exists for Year:{request.Rate.Year} and Week Number:{request.Rate.WeekNumber}"
                };
            }

            // Set Request Date            
            request.Rate.Monday = CalculateMondayDate(request.Rate.Year, request.Rate.WeekNumber);
            request.Rate.Id = Guid.NewGuid();
            request.Rate.Created = DateTime.UtcNow;
            request.Rate.LastModified = null;

            var bookingRateId = await _bookingRateRepository.CreateNewBookingRate(request.Rate);

            if (bookingRateId != Guid.Empty)
            {
                return new RateCommandResult
                {
                    Success = true,
                    RecordsAffected = 1,
                    CommandResult = enums.CommandActionResult.Created,
                    RecordId = bookingRateId
                };
            }

            return new RateCommandResult
            {
                Success = false,
                RecordId = bookingRateId,
                RecordsAffected = 0,
                CommandResult = enums.CommandActionResult.Failure
            };
        }

        private async Task<RateCommandResult> ProcessRateUpdate(BookingRateCommand request)
        {
            var existingRate = await FetchExistingBookingRate(request.Rate);

            if (existingRate == null)
            {
                return new RateCommandResult
                {
                    Success = false,
                    RecordsAffected = 0,
                    CommandResult = enums.CommandActionResult.NotFound,
                    RecordId = request.Rate.Id
                };
            }

            // Set Record Dates - preserve Created Date
            request.Rate.LastModified = DateTime.UtcNow;
            request.Rate.Created = existingRate.Created;
            
            // Set Request Date            
            request.Rate.Monday = CalculateMondayDate(request.Rate.Year, request.Rate.WeekNumber);

            var bookingRateId = await _bookingRateRepository.UpsertBookingRate(request.Rate);

            if (bookingRateId != Guid.Empty)
            {
                return new RateCommandResult
                {
                    Success = true,
                    RecordsAffected = 1,
                    CommandResult = enums.CommandActionResult.Updated,
                    RecordId = bookingRateId
                };
            }

            return new RateCommandResult
            {
                Success = false,
                RecordId = bookingRateId,
                RecordsAffected = 0,
                CommandResult = enums.CommandActionResult.Failure
            };
        }

        private async Task<RateCommandResult> ProcessRateDelete(BookingRateCommand request)
        {
            var existingRate = await FetchExistingBookingRate(request.Rate);
            
            if (existingRate == null)
            {
                return new RateCommandResult
                {
                    Success = false,
                    RecordsAffected = 0,
                    CommandResult = enums.CommandActionResult.NotFound,
                    RecordId = request.Rate.Id
                };
            }           

            var success = await _bookingRateRepository.DeleteBookingRate(request.Rate);

            if (success)
            {
                return new RateCommandResult
                {
                    Success = true,
                    RecordsAffected = 1,
                    CommandResult = enums.CommandActionResult.Deleted,
                    RecordId = request.Rate.Id
                };
            }

            return new RateCommandResult
            {
                Success = false,
                RecordId = request.Rate.Id,
                RecordsAffected = 0,
                CommandResult = enums.CommandActionResult.Failure
            };
        }

        private async Task<BookingRate?> FetchExistingBookingRate(BookingRate rate)
        {
            BookingRate? existingRate = null;            

            try
            {
                existingRate = await _bookingRateRepository.GetBookingRateAsync(rate);
            }
            catch (CosmosException cex)
            when (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Failed to find Booking Rate!");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to find requested Booking Rate with Id/PartitionKey: {rate.Id} and {rate.PartitionKey}");
            }

            return existingRate;
        }



        //public async Task<RateCommandResult> Handle2(BookingRateCommand request, CancellationToken cancellationToken)
        //{
        //    // Set Request Date            
        //    request.NewBookingRate.Monday = CalculateMondayDate(request.NewBookingRate.Year, request.NewBookingRate.WeekNumber);

        //    Guid bookingRateId = Guid.Empty;

        //    var existingRate = await _bookingRateRepository.GetBookingRateAsync(request.NewBookingRate.Year, request.NewBookingRate.WeekNumber);
        //    if (existingRate == null)
        //    {
        //        request.NewBookingRate.Id = Guid.NewGuid();
        //        request.NewBookingRate.Created = DateTime.UtcNow;
        //        request.NewBookingRate.LastModified = null;

        //        bookingRateId = await _bookingRateRepository.CreateNewBookingRate(request.NewBookingRate);
        //    }
        //    else
        //    {
        //        request.NewBookingRate.Id = existingRate.Id;
        //        request.NewBookingRate.Created = existingRate.Created;
        //        request.NewBookingRate.LastModified = DateTime.UtcNow;

        //        bookingRateId = await _bookingRateRepository.UpsertBookingRate(request.NewBookingRate);
        //    }

        //    var result = new RateCommandResult
        //    {
        //        Success = bookingRateId == Guid.Empty ? false : true,
        //        RecordId = bookingRateId
        //    };

        //    return result;
        //}

        private DateTime CalculateMondayDate(int year, int weekNumber)
        {
            Calendar cal = CultureInfo.CurrentCulture.Calendar;
            DateTime jan1 = new DateTime(year, 1, 1);

            int daysOffet = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffet);
            int firstMondayWeekNum = cal.GetWeekOfYear(firstMonday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var firstWeekDay = firstMonday.AddDays((weekNumber - firstMondayWeekNum) * 7);

            return firstWeekDay;
        }
    }
}
