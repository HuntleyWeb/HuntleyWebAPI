namespace HuntleyServicesAPI.Configuration
{
    public interface IApplicationConfiguration
    {
        bool MailEnableSSL { get; }
        string? MailServer { get; }
        string? MailServerPassword { get; }
        string? MailServerUser { get; }
        int PortNumber { get; }
        string? BookingsDbName { get; }
        string? BookingsDbConnection { get; }
        string? BookingRatesContainer { get; }
        string? BookingsContainer { get; }
        int BookingsDbTtl { get; }
    }
}