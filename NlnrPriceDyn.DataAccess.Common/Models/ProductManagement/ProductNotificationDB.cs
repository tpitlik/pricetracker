using System;
using System.Collections.Generic;
using System.Text;

namespace NlnrPriceDyn.DataAccess.Common.Models.ProductManagement
{
    public class ProductNotificationDB
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }
        public bool TriggerOnce { get; set; }
        public float LowPriceLimit { get; set; }
        public string UserId { get; set; }
    }
}
