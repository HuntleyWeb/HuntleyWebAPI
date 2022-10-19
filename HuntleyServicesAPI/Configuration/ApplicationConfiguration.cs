using static System.Environment;

namespace HuntleyServicesAPI.Configuration
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string? MailServer => GetEnvironmentVariable("Mail:Server");

        public string? MailServerUser => GetEnvironmentVariable("Mail:ServerUser");

        public string? MailServerPassword => GetEnvironmentVariable("Mail:ServerPassword");

        public int PortNumber
        {
            get
            {
                int.TryParse(GetEnvironmentVariable("Mail:ServerPort"), out int port);
                return port;
            }
        }

        public bool MailEnableSSL
        {
            get
            {
                bool.TryParse(GetEnvironmentVariable("Mail:EnableSSL"), out bool enabled);
                return enabled;
            }
        }

        public string? BookingsDbName => GetEnvironmentVariable("Data:Bookings:DbName");

        public string? BookingsDbConnection => GetEnvironmentVariable("Data:Bookings:Connection");

        public string? BookingRatesContainer => GetEnvironmentVariable("Data:Bookings:ContainerBookingRates");

        public string? BookingsContainer => GetEnvironmentVariable("Data:Bookings:ContainerBookings");

        public int BookingsDbTtl
        {
            get
            {
                int.TryParse(GetEnvironmentVariable("Data:Bookings:Ttl"), out int ttl);
                return ttl;
            }
        }
    }
}
