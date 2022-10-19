namespace HuntleyServicesAPI.Models
{
    public class CalendarRate
    {
        public string Year { get; set; }

        public string WeekNumber { get; set; }

        public decimal MidWeekRate { get; set; }

        public decimal WeekendRate { get; set; }

        public decimal SevenDayRate { get; set; }

        public bool Available { get; set; }
    }
}
