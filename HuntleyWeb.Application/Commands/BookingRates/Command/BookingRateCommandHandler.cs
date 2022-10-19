using HuntleyWeb.Application.Data.Repos;
using MediatR;
using System.Globalization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Commands.BookingRates.Command
{
    public class BookingRateCommandHandler : IRequestHandler<BookingRateInsertCommand, RateInsertResult>
    {
        private readonly IBookingRateRepository _bookingRateRepository;

        public BookingRateCommandHandler(IBookingRateRepository bookingRateRepository)
        {
            _bookingRateRepository = bookingRateRepository;
        }

        public async Task<RateInsertResult> Handle(BookingRateInsertCommand request, CancellationToken cancellationToken)
        {
            // Set Request Date            
            request.NewBookingRate.Monday = CalculateMondayDate(request.NewBookingRate.Year, request.NewBookingRate.WeekNumber);

            Guid bookingRateId = Guid.Empty;

            var existingRate = await _bookingRateRepository.GetBookingRateAsync(request.NewBookingRate.Year, request.NewBookingRate.WeekNumber);
            if (existingRate == null)
            {
                request.NewBookingRate.Id = Guid.NewGuid();
                request.NewBookingRate.Created = DateTime.UtcNow;
                request.NewBookingRate.LastModified = null;

                bookingRateId = await _bookingRateRepository.CreateNewBookingRate(request.NewBookingRate);
            }
            else
            {
                request.NewBookingRate.Id = existingRate.Id;
                request.NewBookingRate.Created = existingRate.Created;
                request.NewBookingRate.LastModified = DateTime.UtcNow;

                bookingRateId = await _bookingRateRepository.UpsertBookingRate(request.NewBookingRate);
            }

            var result = new RateInsertResult
            {
                Success = bookingRateId == Guid.Empty ? false : true,
                RecordId = bookingRateId
            };

            return result;
        }

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
