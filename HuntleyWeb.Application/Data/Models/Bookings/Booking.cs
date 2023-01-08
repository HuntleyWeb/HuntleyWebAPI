using Newtonsoft.Json;
using System;

namespace HuntleyWeb.Application.Data.Models.Bookings
{
    public class Booking
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        public string PartitionKey => Year.ToString();

        public string BookingType { get; set; }

        public DateTime BookingDate { get; set; }

        public string Cottage { get; set; }

        public bool Confirmed { get; set; }

        public int Days { get; set; }

        public decimal Deposit { get; set; }

        public DateTime DepositPaidDate { get; set; }

        public string EmailAddrress { get; set; }

        public DateTime EndDate { get; set; }

        public string Mobile { get; set; }

        public string Name { get; set; }

        public decimal Rate { get; set; }

        public DateTime StartDate { get; set; }

        public int WeekNumber { get; set; }

        public int Year { get; set; }

        public DateTime Created { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
