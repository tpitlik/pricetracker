using System;
using System.Globalization;
using AutoMapper;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;

namespace NlnrPriceDyn.Logic.Common.Mappings
{
    public class CreateUpdateProductNotificationRequestToProductNotificationDb : Profile
    {
        public CreateUpdateProductNotificationRequestToProductNotificationDb()
        {
            CreateMap<CreateUpdateProductNotificationRequest, ProductNotificationDB>().ForMember(
                dest => dest.LowPriceLimit,
                opt => opt.MapFrom(src => Convert.ToSingle(src.LowPriceLimit, CultureInfo.InvariantCulture)));
        }
    }
}
