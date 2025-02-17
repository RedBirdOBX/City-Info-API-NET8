namespace CityInfoAPI.Web.Services
{
    public class LocalMailService
    {
        private string _mailTo = "admin@email.com";
        private string _mailFrom = "no-reply@email.com";

        public void Send(string subject, string message)
        {
            // fake method to emulate sending mail
            Console.WriteLine($"    *Mail from {_mailFrom} to {_mailTo}, with LocalMailService.");
            Console.WriteLine($"    *Subject: {subject}");
            Console.WriteLine($"    *Message: {message}");
        }
    }
}
