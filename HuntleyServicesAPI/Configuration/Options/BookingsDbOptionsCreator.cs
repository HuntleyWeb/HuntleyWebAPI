using HuntleyWeb.Application.Configuration;

namespace HuntleyServicesAPI.Configuration.Options
{
    public class BookingsDbOptionsCreator
    {
        public static CosmosOptions Create(IApplicationConfiguration applicationConfiguration) => new()
        {
            ConnectionString = applicationConfiguration.BookingsDbConnection,
            ContainerName = applicationConfiguration.BookingsContainer,
            DatabaseName = applicationConfiguration.BookingsDbName,
            TtlSecs = applicationConfiguration.BookingsDbTtl          
        };
    }
}