using Newtonsoft.Json;
using System;

namespace HuntleyWeb.Application.Data.Models.Bookings
{
    public class BookingRate
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        public string PartitionKey => Year.ToString();

        public int Year { get; set; }

        public int WeekNumber { get; set; }

        public bool AvailableForRental { get; set; } = false;

        public decimal MidWeekRate {  get; set; }

        public decimal WeekendRate {  get; set; }

        public decimal SevenDayRate {  get; set; }

        public DateTime Monday {  get; set; }

        public DateTime Created { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
