namespace HuntleyWeb.Application.Configuration
{
    public class MailOptions
    {
        public string Server { get; set; }

        public string ServerPassword { get; set; }

        public string MailUser { get; set; }

        public int PortNumber {  get; set; }

        public bool EnableSSL { get; set; }
    }
}
