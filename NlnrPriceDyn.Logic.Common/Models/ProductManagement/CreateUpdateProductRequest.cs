using System;
using System.Collections.Generic;
using System.Text;

namespace NlnrPriceDyn.Logic.Common.Models.ProductManagement
{
    public class CreateUpdateProductRequest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string LinkUrl { get; set; }
        public string UserId { get; set; }
    }
}
