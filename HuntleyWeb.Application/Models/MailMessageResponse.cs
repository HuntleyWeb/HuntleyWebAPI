using System;

namespace HuntleyWeb.Application.Models
{
    public class MailMessageResponse
    {
        public MailMessageResponse()
        {
            Created = DateTime.UtcNow;
        }

        public bool Success { get; set; }

        public string Information { get; set; }

        public DateTime Created { get; }

        public DateTime Sent { get; set; }
    }
}
