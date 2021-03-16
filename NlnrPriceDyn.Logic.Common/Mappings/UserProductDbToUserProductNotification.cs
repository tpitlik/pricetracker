using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;

namespace NlnrPriceDyn.Logic.Common.Mappings
{
    public class UserProductDbToUserProductNotification: Profile
    {
        public UserProductDbToUserProductNotification()
        {
            CreateMap<UserProductDB, UserProductNotification>()
                .ForMember(p => p.Id, opt => opt.MapFrom(src => src.ProductNotificationDb.Id))
                .ForMember(p => p.ProductTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(p => p.CurrentMinPrice, opt => opt.MapFrom(src => src.ProductDb.MinPrice))
                .ForMember(p => p.IsActive, opt => opt.MapFrom(src => src.ProductNotificationDb.IsActive))
                .ForMember(p => p.LowPriceLimit, opt => opt.MapFrom(src => src.ProductNotificationDb.LowPriceLimit))
                .ForMember(p => p.TriggerOnce, opt => opt.MapFrom(src => src.ProductNotificationDb.TriggerOnce));
        }
    }
}
