using FluentAssertions;
using HuntleyWeb.Application.Models;
using HuntleyWeb.Application.Services.Email;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Net.Mail;
using Xunit;

namespace UnitTestMail.Services
{
    public class SmtpServiceTests
    {
        private static string _host = "mail5.hostinguk.net";
        private static int _port = 0;
        private static string _mailUserName = "jon.huntley@huntleyweb.co.uk";
        private static string _mailUserPwd = "Scooter25@";

        [Fact]
        public void TestMailSend()
        {
            var smtpClient = new SmtpClient(_host, _port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_mailUserName, _mailUserPwd),
                EnableSsl = false
            };

            var content = "<b>Hello Jon</b><br/>What you doing?";
            var service = new MailMessageService(smtpClient, NullLogger<MailMessageService>.Instance);

            var request = new MailMessageRequest
            {
                FromAddress = "jon.huntley@huntleyweb.co.uk",
                TargetAddress = "jon.huntley@hotmail.co.uk",
                Subject = "Test Message",
                MessageBody = content,
                NotificationList = ""
            };

            var result = service.SendMailMessage(request);

            result.Should().NotBeNull();
        }
    }
}
