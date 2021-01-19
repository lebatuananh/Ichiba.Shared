using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface IEmailService
    {
        Task SendAsync(string smtpHost, int smtpPort, string smtpUser, string smtpPass, string from, string to,
            string subject, string html, List<FileAttachment> formFiles, string[] cc, string[] bcc, string senderTitle);
    }
}