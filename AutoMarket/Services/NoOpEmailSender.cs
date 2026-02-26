using Microsoft.AspNetCore.Identity.UI.Services;

namespace AutoMarket.Services
{
    public class NoOpEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
           
            return Task.CompletedTask;
        }
    }
}
