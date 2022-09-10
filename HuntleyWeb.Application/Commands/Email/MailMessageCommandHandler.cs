using HuntleyWeb.Application.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Commands.Email
{
    public class MailMessageCommandHandler : IRequestHandler<MailMessageCommand, MailMessageResult>
    {
        private readonly IMailMessageService _mailService;

        public MailMessageCommandHandler(IMailMessageService mailService)
        {
            _mailService = mailService;
        }

        public async Task<MailMessageResult> Handle(MailMessageCommand request, CancellationToken cancellationToken)
        {
            var response = await _mailService.SendMailMessage(request.MessageRequest);

            var result = new MailMessageResult
            {
                Response = response
            };

            return result;
        }
    }
}
