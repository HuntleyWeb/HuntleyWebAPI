using System;
using System.Text.Json.Serialization;

namespace HuntleyWeb.Application.Data.Models.Requests
{
    public class DateRangeRequest
    {
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
    }
}
