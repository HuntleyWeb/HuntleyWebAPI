namespace HuntleyWeb.Application.Models
{
    public class MailMessageRequest
    {
        public MailMessageRequest()
        {
            BodyIsHtml = true;
        }        

        public string FromAddress { get; set; }

        public string TargetAddress { get; set; }

        public string Subject { get; set; }

        public string MessageBody { get; set; }

        public string NotificationList { get; set; }

        public bool BodyIsHtml { get; set; }
    }
}
