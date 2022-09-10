using HuntleyWeb.Application.Models;
using MediatR;

namespace HuntleyWeb.Application.Commands.Email
{
    public class MailMessageCommand : IRequest<MailMessageResult>
    {
        public MailMessageRequest MessageRequest { get; set; }
    }
}
