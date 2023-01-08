using HuntleyWeb.Application.Commands.enums;
using HuntleyWeb.Application.Data.Models.Bookings;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace HuntleyWeb.Application.Services.Bookings
{
    public static class BookingsHelper
    {
        public const int WeekEndBreakDuration = 3;
        public const int MidWeekBreakDuration = 4;
        public const int SevenDayBreakDuration = 7;

        public static int GetBookingWeekNumber(DateTime startDate)
        {
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(startDate);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                startDate = startDate.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(startDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static (string, bool) BookingsDatesAreValid(DateTime startDate, DateTime endDate, int noticeDays = 7)
        {
            var minDate = DateTime.Now.AddDays(noticeDays);

            if (startDate < minDate)
                return ("Start Date is too soon!", false);

            if (endDate <= startDate) 
                return ("Booking End Date cannot be less than Booking Start Date!", false);

            var diffDays = (endDate - startDate).TotalDays;

            if (diffDays <= 3)
                return ("Booking Period is too short, it cannot be less than 3 days!", false);

            return ("All Good!", true);
        }

        public static (int, BreakType) GetBookingType(DateTime startDate, DateTime endDate)
        {
            var duration = GetDurationInDays(startDate, endDate);

            if (startDate.DayOfWeek == DayOfWeek.Saturday && duration == SevenDayBreakDuration)
            {
                return (duration, BreakType.SevenDay);
            }

            if (startDate.DayOfWeek == DayOfWeek.Friday && duration == WeekEndBreakDuration)
            {
                return (duration, BreakType.Weekend);
            }

            if (startDate.DayOfWeek == DayOfWeek.Monday && duration == MidWeekBreakDuration)
            {
                return (duration, BreakType.MidWeek);
            }

            if (duration > 0)
            {
                return(duration, BreakType.Custom);
            }

            return (duration, BreakType.Undefined);
        }

        public static int GetDurationInDays(DateTime startDate, DateTime endDate)
        {
            var diffDays = (endDate - startDate).TotalDays;

            return (int)diffDays;
        }

        public static string[] GetBookedDays(List<Booking> bookings)
        {
            var BookedDayFormat = "dd/MM/yyyy";

            var days = new List<string>();

            foreach(Booking booking in bookings)
            {
                var startDate = booking.StartDate;
                var endDate = booking.EndDate;

                for (int i = 0; i <= booking.Days; i++)
                {
                    var bookedDate = startDate.AddDays(i);

                    days.Add(bookedDate.ToString(BookedDayFormat));
                }
            }

            return days.ToArray();
        }
    }
}
