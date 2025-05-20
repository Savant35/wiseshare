
namespace Wiseshare.Application.Common.Interfaces.Email;
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }

