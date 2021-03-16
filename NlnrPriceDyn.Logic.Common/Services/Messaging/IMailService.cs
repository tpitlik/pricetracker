using System.Threading.Tasks;
using NlnrPriceDyn.Logic.Common.Models.Messaging;

namespace NlnrPriceDyn.Logic.Common.Services.Messaging
{
    public interface IMailService
    {
        Task SendAsync(EmailMessage message);
    }
}
