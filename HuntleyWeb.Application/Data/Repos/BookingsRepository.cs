using HuntleyWeb.Application.Configuration;
using HuntleyWeb.Application.Data.Models.Bookings;
using HuntleyWeb.Application.Data.Repos.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Data.Repos
{
    public class BookingsRepository : CosmosRepository<Booking>, IBookingsRepository
    {        
        private const string DateFormat = "yyyy-MM-dd";

        public BookingsRepository(CosmosClient cosmosClient, IOptions<BookingOptions> options)
            : base(cosmosClient, options.Value.DatabaseName, options.Value.ContainerName)
        {
        }

        public async Task<Guid> CreateNewBooking(Booking booking)
        {
            var result = await base.CreateAsync(booking, booking.PartitionKey);

            if (result != null)
                return result.Id;

            return Guid.Empty;
        }

        public async Task<Booking> GetBookingAsync(Booking booking)
        {
            var result = await base.GetDocumentAsync(booking.Id.ToString(), booking.PartitionKey);

            return result;
        }

        public async Task<bool> DeleteBooking(Booking booking)
        {
            var result = await base.DeleteDocumentAsync(booking.Id.ToString(), booking.PartitionKey);

            return result;
        }

        public async Task<Booking> GetBookingAsync(DateTime startDate, DateTime endDate)
        {
            // WHERE c.StartDate >=  '2023-01-23' AND c.StartDate <= '2023-01-27'

            var query = $"Select * FROM c WHERE c.StartDate >= '{startDate.ToString(DateFormat)}' AND c.StartDate <= '{endDate.ToString(DateFormat)}' ORDER BY c.Created DESC";

            var result = await base.QuerySingleAsync(query);

            return result;
        }

        public async Task<List<Booking>> GetBookingsAsync(DateTime startDate, DateTime endDate)
        {
            // WHERE c.StartDate >= '2022-12-01' AND c.EndDate <= '2024-01-31'

            var query = $"Select * FROM c WHERE c.StartDate >= '{startDate.ToString(DateFormat)}' AND c.EndDate <= '{endDate.ToString(DateFormat)}' ORDER BY c.Created DESC";

            var result = await base.RunQuery(query);

            return result;
        }
    }
}
