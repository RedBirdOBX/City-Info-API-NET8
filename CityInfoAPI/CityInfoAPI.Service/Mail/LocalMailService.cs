using Microsoft.Extensions.Configuration;

namespace CityInfoAPI.Service;

/// <summary>
/// Emulated mail service; local implementation.
/// </summary>
public class LocalMailService : IMailService
{
    private string _mailTo = string.Empty;
    private string _mailFrom = string.Empty;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="configuration"></param>
    public LocalMailService(IConfiguration configuration)
    {
        _mailTo = configuration["MailSettings:mailToAddress"] ?? string.Empty;
        _mailFrom = configuration["MailSettings:mailFromAddress"]  ?? string.Empty;
    }

    /// <summary>
    /// send mail message
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    public void Send(string subject, string message)
    {
        // fake method to emulate sending mail
        Console.WriteLine($"    *Mail from {_mailFrom} to {_mailTo}, with LocalMailService.");
        Console.WriteLine($"    *Subject: {subject}");
        Console.WriteLine($"    *Message: {message}");
    }
}
