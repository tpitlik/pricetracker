using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AutoMapper;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;

namespace NlnrPriceDyn.Logic.Common.Mappings
{
    public class UserProductDbToUserProduct: Profile
    {
        public UserProductDbToUserProduct()
        {
            CreateMap<UserProductDB, UserProduct>()
                .ForMember(p => p.MinPrice,
                    opt => opt.MapFrom(src =>
                        src.ProductDb.MinPrice.ToString("####0.00", CultureInfo.InvariantCulture)))
                .ForMember(p => p.MaxPrice,
                    opt => opt.MapFrom(src =>
                        src.ProductDb.MaxPrice.ToString("####0.00", CultureInfo.InvariantCulture)))
                .ForMember(p => p.MeanPrice,
                    opt => opt.MapFrom(
                        src => src.ProductDb.MeanPrice.ToString("####0.00", CultureInfo.InvariantCulture)))
                .ForMember(p => p.MinPriceChange,
                    opt => opt.MapFrom(
                        src => src.ProductDb.MinPriceChange == 0
                            ? "--"
                            : src.ProductDb.MinPriceChange.ToString("+####0.00; -####0.00",
                                CultureInfo.InvariantCulture)))
                .ForMember(p => p.MaxPriceChange,
                    opt => opt.MapFrom(
                        src => src.ProductDb.MaxPriceChange == 0
                            ? "--"
                            : src.ProductDb.MaxPriceChange.ToString("+####0.00; -####0.00",
                                CultureInfo.InvariantCulture)))
                .ForMember(p => p.MeanPriceChange,
                    opt => opt.MapFrom(
                        src => src.ProductDb.MeanPriceChange == 0
                            ? "--"
                            : src.ProductDb.MeanPriceChange.ToString("+####0.00; -####0.00",
                                CultureInfo.InvariantCulture)))
                .ForMember(p => p.OffersCount, opt => opt.MapFrom(src => src.ProductDb.OffersCount))
                .ForMember(p => p.ProductNotificationActive,
                    opt => opt.MapFrom(src => src.ProductNotificationDb.IsActive))
                .ForMember(p => p.ProductNotificationId,
                    opt => opt.MapFrom(src => src.ProductNotificationDb.Id))
                .ForMember(p => p.ProductNotificationTriggerOnce,
                    opt => opt.MapFrom(src => src.ProductNotificationDb.TriggerOnce))
                .ForMember(p => p.OutOfStock, opt => opt.MapFrom(src => src.ProductDb.OutOfStock));
        }
    }
}
