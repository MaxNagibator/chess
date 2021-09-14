namespace Bg.Chess.Web
{
    using MailKit.Security;

    public class MailSenderConfig
    {
        public bool SaveLocal { get; set; }
        public SenderValue[] Value { get; set; }

        public class SenderValue
        {
            public string Name { get; set; }
            public string SmptAddress { get; set; }
            public int Port { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public SecureSocketOptions? SecureSocketOptions { get; set; }
        }
    }
}
