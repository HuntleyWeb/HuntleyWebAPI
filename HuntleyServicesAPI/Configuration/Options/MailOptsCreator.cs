using HuntleyWeb.Application.Configuration;

namespace HuntleyServicesAPI.Configuration.Options
{
    public class MailOptsCreator : BaseOptionsCreator<MailOptions>
    {
        public override MailOptions Create(IApplicationConfiguration applicationConfiguration)
        {
            var mailOptions = new MailOptions()
            {
                MailUser = applicationConfiguration.MailServerUser,
                Server = applicationConfiguration.MailServer,
                ServerPassword = applicationConfiguration.MailServerPassword,
                PortNumber = applicationConfiguration.PortNumber,
                EnableSSL = applicationConfiguration.MailEnableSSL
            };

            return mailOptions;
        }  
    }
}
