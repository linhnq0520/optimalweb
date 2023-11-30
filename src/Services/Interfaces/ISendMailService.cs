namespace optimalweb.Services.Interfaces
{
    public interface ISendMailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendSmsAsync(string number, string message);
    }
}
