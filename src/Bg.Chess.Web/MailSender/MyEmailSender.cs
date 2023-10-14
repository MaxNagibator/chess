
namespace Bg.Chess.Web
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bg.Chess.Common;

    using MailKit.Net.Smtp;
    using MailKit.Security;

    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using MimeKit;

    public class MyEmailSender : IEmailSender
    {
        private MailSenderConfig _options;
        private ILogger _logger;
        private ILogger _mailLocalSaver;

        public MyEmailSender(IOptions<MailSenderConfig> optionsAccessor,
            ILoggerFactory loggerFactory)
        {
            _options = optionsAccessor.Value;

            _logger = loggerFactory.CreateLogger("mailSender");
            if (_options.SaveLocal)
            {
                _mailLocalSaver = loggerFactory.CreateLogger("mailLocalSaver");
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (_options.SaveLocal)
            {
                // чтоб ссылка с логов корректно открывалась
                htmlMessage = htmlMessage.Replace("&amp;", "&");
                _mailLocalSaver.LogInformation("send mail to " + email + "\r\n" + subject + "\r\n" + htmlMessage);
                return;
            }

            if (_options.Value.Length == 0)
            {
                _mailLocalSaver.LogWarning("Empty value");
                return;
            }

            var senders = _options.Value.OrderBy(x => Guid.NewGuid()).ToList();

            foreach (var sender in senders)
            {
                try
                {
                    var emailMessage = new MimeMessage();

                    emailMessage.From.Add(new MailboxAddress("Администрация шахматишек", sender.Login));
                    emailMessage.To.Add(new MailboxAddress("Уважаемый шахматист", email));
                    emailMessage.Subject = subject;
                    emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = htmlMessage
                    };

                    using (var client = new SmtpClient())
                    {
                        client.Timeout = 5000;

                        if (sender.SecureSocketOptions != null)
                        {
                            await client.ConnectAsync(sender.SmptAddress, sender.Port, sender.SecureSocketOptions.Value);
                        }
                        else
                        {
                            await client.ConnectAsync(sender.SmptAddress, sender.Port, false);
                        }

                        await client.AuthenticateAsync(sender.Login, sender.Password);
                        await client.SendAsync(emailMessage);
                        await client.DisconnectAsync(true);
                    }

                    return;
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex, sender.Name + " sending error");
                }
            }

            throw new Exception("all senders fail sending");
        }
    }
}
