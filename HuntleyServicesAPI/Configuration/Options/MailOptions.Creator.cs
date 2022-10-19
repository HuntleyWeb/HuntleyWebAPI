using HuntleyWeb.Application.Configuration;

namespace HuntleyServicesAPI.Configuration.Options
{
    public class MailOptionsCreator
    {
        public static MailOptions Create(IApplicationConfiguration applicationConfiguration) => new()
        {                       
            MailUser = applicationConfiguration.MailServerUser,
            Server = applicationConfiguration.MailServer,
            ServerPassword = applicationConfiguration.MailServerPassword,
            PortNumber = applicationConfiguration.PortNumber,
            EnableSSL = applicationConfiguration.MailEnableSSL            
        };       
    }
}
