using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookWebHooks
{
    public static class Mail
    {
        public static void SendMail(MailOptions options, string html)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(options.FromName, options.FromMail));
            message.To.Add(new MailboxAddress(options.ToName, options.ToMail));
            message.Subject = "Facebook WebHooks";

            var builder = new BodyBuilder();
            builder.HtmlBody = html ?? "No content";

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(options.SmtpHost, options.SmtpPort, options.SmtpUseSsl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(options.SmtpLogin, options.SmtpPassword);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
