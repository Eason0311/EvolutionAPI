namespace prjtestAPI.Services.Interfaces
{
    public interface IMailService
    {
        Task SendAsync(string toEmail, string subject, string htmlContent);
    }
}
