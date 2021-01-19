using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Shared.Services
{
    public class EmailSenderService : IEmailService
    {
        public async Task SendAsync(string smtpHost, int smtpPort, string smtpUser, string smtpPass, string from,
            string to, string subject, string html, List<FileAttachment> formFiles, string[] cc, string[] bcc,
            string senderTitle)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(senderTitle, from));
            email.To.Add(MailboxAddress.Parse(to));
            if (cc != null && cc.Length > 0)
                foreach (var item in cc)
                {
                    var replace = item.Replace("[\"", "");
                    var s = replace.Replace("\"]", "");
                    email.Cc.Add(MailboxAddress.Parse(s));
                }

            if (bcc != null && bcc.Length > 0)
                foreach (var item in bcc)
                {
                    var replace = item.Replace("[\"", "");
                    var s = replace.Replace("\"]", "");
                    email.Bcc.Add(MailboxAddress.Parse(s));
                }

            email.Subject = subject;
            var builder = new BodyBuilder();
            if (formFiles != null)
                foreach (var file in formFiles)
                    builder.Attachments.Add(file.FileName, file.Bytes, ContentType.Parse(file.ContentType));

            builder.HtmlBody = html;
            email.Body = builder.ToMessageBody();
            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpHost, smtpPort,
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}