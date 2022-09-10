using HuntleyWeb.Application.Models;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Services
{
    public interface IMailMessageService
    {
        Task<MailMessageResponse> SendMailMessage(MailMessageRequest request);
    }
}