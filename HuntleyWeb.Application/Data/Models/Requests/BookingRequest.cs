using System;
using System.Text.Json.Serialization;

namespace HuntleyWeb.Application.Data.Models.Requests
{
    public class BookingRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("emailAddress")]
        public string EmailAddrress { get; set; }

        [JsonPropertyName("mobile")]
        public string Mobile { get; set; }

        [JsonPropertyName("cottage")]
        public string Cottage { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }
}
