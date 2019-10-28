namespace Dios.Helpers
{
    public class NetworkCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class EmailSettings
    {
        public string MailServer { get; set; }
        public NetworkCredentials NetworkCredentials { get; set; }

        public int Port { get; set; }
        public bool EnableSsl { get; set; }

        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Bcc{ get; set; }
        public string ReplyToEmail { get; set; }
        public string ReplyToName { get; set; }
    }
}
