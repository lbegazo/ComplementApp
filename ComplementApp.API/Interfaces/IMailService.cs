using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendWelcomeEmailAsync(WelcomeRequest request);
    }
}