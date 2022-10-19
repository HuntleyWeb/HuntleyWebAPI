using HuntleyServicesAPI.Models;
using HuntleyWeb.Application.Commands.Email;
using HuntleyWeb.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace HuntleyServicesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMediator mediator, IConfiguration configuration, ILogger<MessageController> logger)
        {            
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [Route("mailuser")]
        public async Task<IActionResult> GetMailServerUser()
        {
            var mailUser = _configuration.GetValue<string>("MailServerUser");

            _logger.LogInformation("Called");

            return Ok(mailUser);
        }

        [HttpPost]
        [Route("sendmessage")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Successfully Sent Email Message")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SendMailMessage([FromBody] ContactMessage emailDetails)
        {
            if (string.IsNullOrEmpty(emailDetails.TargetAddress) || string.IsNullOrEmpty(emailDetails.FromAddress))
            {
                return BadRequest("Missing Email Address");
            }

            if (string.IsNullOrEmpty(emailDetails.Subject))
            {
                return BadRequest("Missing Message Subject");
            }

            var mailRequest = new MailMessageRequest
            {
                FromAddress = emailDetails.FromAddress,
                TargetAddress = emailDetails.TargetAddress,
                Subject = emailDetails.Subject,
                MessageBody = emailDetails.Content,
                NotificationList = emailDetails.NotificationList
            };

            var command = new MailMessageCommand
            {
                MessageRequest = mailRequest
            };

            var result = await _mediator.Send(command);

            return Ok(result.Response);
        }
    }
}
