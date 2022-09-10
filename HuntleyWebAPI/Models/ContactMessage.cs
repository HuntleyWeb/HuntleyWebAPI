namespace HuntleyWebAPI.Models
{
    public class ContactMessage
    {
        public string FromAddress { get; set; }
        public string TargetAddress { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string NotificationList { get; set; }
        public bool BodyIsHtml { get; set; }
    }
}
