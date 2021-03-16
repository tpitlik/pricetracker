using System;
using System.Collections.Generic;
using System.Text;

namespace NlnrPriceDyn.DataAccess.Common.Models.ProductManagement
{
    public class UserProductDB
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public string LinkUrl { get; set; }
        public string CatalogueId { get; set; }
        public string CatalogueName { get; set; }
        public DateTime Added { get; set; }
        public ProductDB ProductDb { get; set; }
        public ProductNotificationDB ProductNotificationDb { get; set; }
    }
}
