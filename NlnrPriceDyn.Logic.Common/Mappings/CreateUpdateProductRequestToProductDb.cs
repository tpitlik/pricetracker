using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;

namespace NlnrPriceDyn.Logic.Common.Mappings
{
    public class CreateUpdateProductRequestToProductDb : Profile
    {
        public CreateUpdateProductRequestToProductDb()
        {
            CreateMap<CreateUpdateProductRequest, ProductDB>();
        }
    }
}
