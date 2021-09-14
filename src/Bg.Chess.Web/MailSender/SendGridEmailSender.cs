namespace Bg.Chess.Web
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using SendGrid;
    using SendGrid.Helpers.Mail;

    public class SendGridEmailSender : IEmailSender
    {
        private SendGridConfig _options;
        private ILogger _logger;
        private ILogger _mailLocalSaver;

        public SendGridEmailSender(IOptions<SendGridConfig> optionsAccessor, ILoggerFactory loggerFactory)
        {
            _options = optionsAccessor.Value;

            _logger = loggerFactory.CreateLogger("mailSender");
            if (_options.SaveLocal)
            {
                _mailLocalSaver = loggerFactory.CreateLogger("mailLocalSaver");
            }
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await Execute(_options.Key, subject, message, email);
        }

        public async Task Execute(string apiKey, string subject, string message, string email)
        {
            message = message.Replace("https://localhost:44366", "http://chess.bob217.ru");
            if (_options.SaveLocal)
            {
                _mailLocalSaver.LogInformation("send mail to " + email + "\r\n" + subject + "\r\n" + message);
                return;
            }

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("chess.bob217@mail.ru", "Администрация шахматишек"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            var response = await client.SendEmailAsync(msg);
            var test = await response.Body.ReadAsStringAsync();
            var test2 = response.StatusCode;
        }
    }
}
