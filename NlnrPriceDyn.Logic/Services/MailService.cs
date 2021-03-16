using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NlnrPriceDyn.Logic.Common.Models.Messaging;
using NlnrPriceDyn.Logic.Common.Services.Messaging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NlnrPriceDyn.Logic.Services
{
    public class MailService: IMailService
    {
        readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendAsync(EmailMessage message)
        {
            return СonfigSendGridAsync(message);
        }

        private async Task СonfigSendGridAsync(EmailMessage message)
        {
            var apiKey = _configuration.GetValue<string>("SendGrid:ApiKey");
            var mailAddr = _configuration.GetValue<string>("SendGrid:NoreplyMailAddr");
            var fromStr = _configuration.GetValue<string>("App:Name");

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(mailAddr, fromStr);
            var to = new EmailAddress(message.Destination);
            var subject = message.Subject;
            var htmlContent = message.Body;
            var plainTextContent = message.Body;
            //var plainTextContent = Regex.Replace(htmlContent, "<[^>]*>", "");

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
