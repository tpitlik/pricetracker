using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NlnrPriceDyn.Logic.Common.Exceptions
{
    [Serializable]
    public class NlnrWebApiException : Exception
    {
        public NlnrWebApiException()
        {
        }

        public NlnrWebApiException(string message) : base(message)
        {
        }

        public NlnrWebApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NlnrWebApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
