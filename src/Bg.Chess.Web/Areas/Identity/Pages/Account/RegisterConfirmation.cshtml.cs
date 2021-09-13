using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Bg.Chess.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayConfirmAccountLink = false;// true;
            //if (DisplayConfirmAccountLink)
            //{
               var userId = await _userManager.GetUserIdAsync(user);
               var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
               code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            //}
            await SendMail();

            return Page();
        }

        private async Task SendMail()
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация шахматишек", "chess.bob217@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", Email));
            emailMessage.Subject = "Подтверждение регистрации";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<a id = \"confirm-link\" href = \""+EmailConfirmationUrl+"\" > Перейдите по этой ссылке, для подтверждения регистрации на chess.bob217.ru</ a > "
            };

            using (var client = new SmtpClient())
            {
                ////////await client.ConnectAsync("smtp.mail.ru", 25, false);//, MailKit.Security.SecureSocketOptions.StartTls);
                ////////var test0 = "H2GAJ4rujpt$";
                ////////var test1 = "Аутлук на домашнем компьютере";
                ////////var test2 = "h8zSCfw60C65JSSbg5jc";
                ////////await client.AuthenticateAsync("chess.bob217@mail.ru", test2);
                client.Timeout = 5000;

                await client.ConnectAsync("smtp.yandex.ru", 25, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("chess.bob217@yandex.ru", "asd1;l1m231hasd8&Asd123");

                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

        }
    }
}
