using HuntleyWeb.Application.Configuration;

namespace HuntleyServicesAPI.Configuration.Options
{
    public class BookingsRatesDbOptionsCreator
    {
        public static CosmosOptions Create(IApplicationConfiguration applicationConfiguration) => new()
        {
            ConnectionString = applicationConfiguration.BookingsDbConnection,
            ContainerName = applicationConfiguration.BookingRatesContainer,
            DatabaseName = applicationConfiguration.BookingsDbName,
            TtlSecs = applicationConfiguration.BookingsDbTtl
        };
    }
}
