namespace NlnrPriceDyn.Logic.Common.Models.ProductManagement
{
    public class CreateUpdateProductNotificationRequest
    {
        public string Id { get; set; }
        public string LowPriceLimit { get; set; }
        public bool IsActive { get; set; }
        public bool TriggerOnce { get; set; }
    }
}
