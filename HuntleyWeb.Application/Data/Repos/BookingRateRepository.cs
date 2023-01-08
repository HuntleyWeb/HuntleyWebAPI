using HuntleyWeb.Application.Configuration;
using HuntleyWeb.Application.Data.Models.Bookings;
using HuntleyWeb.Application.Data.Repos.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Data.Repos
{
    public class BookingRateRepository : CosmosRepository<BookingRate>, IBookingRateRepository
    {
        public BookingRateRepository(CosmosClient cosmosClient, IOptions<BookingRateOptions> options)
            : base(cosmosClient, options.Value.DatabaseName, options.Value.ContainerName)
        {
        }

        public async Task<Guid> CreateNewBookingRate(BookingRate rate)
        {
            var result = await base.CreateAsync(rate, rate.PartitionKey);

            if (result != null)
                return result.Id;

            return Guid.Empty;
        }

        public async Task<Guid> UpsertBookingRate(BookingRate rate)
        {
            var result = await base.UpsertAsync(rate, rate.PartitionKey);

            if (result != null)
                return result.Id;

            return Guid.Empty;
        }

        public async Task<BookingRate> GetBookingRateAsync(BookingRate rate)
        {
            var result = await base.GetDocumentAsync(rate.Id.ToString(), rate.PartitionKey);

            return result;
        }

        public async Task<bool> DeleteBookingRate(BookingRate rate)
        {
            var result = await base.DeleteDocumentAsync(rate.Id.ToString(), rate.PartitionKey);
        
            return result;
        }

        public async Task<BookingRate> GetBookingRateAsync(int year, int weekNumber)
        {
            var query = $"Select * FROM c WHERE c.Year = {year} AND c.WeekNumber = {weekNumber} ORDER BY c.Created DESC";

            var result = await base.QuerySingleAsync(query);

            return result;
        }
    }
}
