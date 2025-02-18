namespace CityInfoAPI.Web.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = string.Empty;
        private string _mailFrom = string.Empty;

        public CloudMailService(IConfiguration configuration)
        {
            _mailTo = configuration["MailSettings:mailToAddress"] ?? string.Empty;
            _mailFrom = configuration["MailSettings:mailFromAddress"]  ?? string.Empty;
        }

        public void Send(string subject, string message)
        {
            // fake method to emulate sending mail
            Console.WriteLine($"    *Mail from {_mailFrom} to {_mailTo}, with CloudMailService.");
            Console.WriteLine($"    *Subject: {subject}");
            Console.WriteLine($"    *Message: {message}");
        }
    }
}
