namespace CityInfoAPI.Service;

/// <summary>
/// Emulated mail service interface
/// </summary>
public interface IMailService
{
    /// <summary>
    /// method to send mail message
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    void Send(string subject, string message);
}