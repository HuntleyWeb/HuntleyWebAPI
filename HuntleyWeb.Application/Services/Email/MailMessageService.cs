using HuntleyWeb.Application.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Services.Email
{
    public class MailMessageService : IMailMessageService
    {
        private readonly SmtpClient _smtpClient;
        private readonly ILogger<MailMessageService> _logger;

        public MailMessageService(SmtpClient mailClient, ILogger<MailMessageService> logger)
        {
            _smtpClient = mailClient;
            _logger = logger;
        }

        public async Task<MailMessageResponse> SendMailMessage(MailMessageRequest request)
        {
            var response = new MailMessageResponse();

            try
            {
                using (MailMessage mailmessage = new MailMessage())
                {
                    mailmessage.From = new MailAddress(request.FromAddress);
                    mailmessage.To.Add(request.TargetAddress);
                    mailmessage.Subject = request.Subject;
                    mailmessage.IsBodyHtml = request.BodyIsHtml;
                    mailmessage.Body = request.MessageBody;

                    using var logScope = _logger.BeginScope(new Dictionary<string, object>
                    {
                        ["FromAddress"] = request.FromAddress,
                        ["TargetAddress"] = request.TargetAddress,
                        ["NotificationList"] = !string.IsNullOrEmpty(request.NotificationList) ? request.NotificationList : "Empty Notification List"
                    });

                    if (!string.IsNullOrEmpty(request.NotificationList))
                    {
                        foreach (string recipient in request.NotificationList.Split(';'))
                        {
                            if (!string.IsNullOrEmpty(recipient))
                            {
                                mailmessage.CC.Add(recipient);
                            }
                        }

                        _logger.LogInformation($"Included Notification List of {mailmessage.CC.Count} Email Addresses!");
                    }

                    await Task.Run(() => _smtpClient.Send(mailmessage));

                    response.Information = $"Successfully sent mail message to {request.TargetAddress}";
                    response.Sent = DateTime.UtcNow;
                    response.Success = true;

                    _logger.LogInformation("Successfully sent mail Message!");
                }
            }
            catch (Exception ex)
            {
                using var errorlogScope = _logger.BeginScope(new Dictionary<string, object>
                {
                    ["Subject"] = request.Subject,
                    ["ErrorMessage"] = ex.Message
                });

                _logger.LogError(ex, "Failed to send Mail Message!");
                throw;
            }
            finally
            {
            }

            return response;
        }
    }
}
