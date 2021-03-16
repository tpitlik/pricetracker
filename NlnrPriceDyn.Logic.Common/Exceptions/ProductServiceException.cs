using System;
using System.Runtime.Serialization;
using NlnrPriceDyn.Logic.Common.Exceptions;

namespace NlnrPriceDyn.Logic.Common
{
    [Serializable]
    public class ProductServiceException : NlnrWebApiException
    {
        public ProductServiceException()
        {
        }

        public ProductServiceException(string message) : base(message)
        {
        }

        public ProductServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProductServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}