using System;
using System.Collections.Generic;
using System.Text;

namespace NlnrPriceDyn.Logic.Common.Models.Messaging
{
    public class EmailMessage
    {
        public string Destination { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
