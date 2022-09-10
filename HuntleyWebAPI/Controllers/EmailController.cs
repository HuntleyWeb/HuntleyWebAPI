using HuntleyWeb.Application.Commands.Email;
using HuntleyWeb.Application.Models;
using HuntleyWeb.Application.Services;
using HuntleyWebAPI.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace HuntleyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMailMessageService _mailService;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailController> _logger;

        //public EmailController(IMailMessageService mailService, IConfiguration configuration, ILogger<EmailController> logger)
        public EmailController(IMediator mediator, IConfiguration configuration, ILogger<EmailController> logger)
        {
            //_mailService = mailService;
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
        public async Task<IActionResult> SendMailMessage([FromBody]ContactMessage emailDetails)
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

            //var result = await _mailService.SendMailMessage(mailRequest);
           
            return Ok(result.Response);
        }
    }
}