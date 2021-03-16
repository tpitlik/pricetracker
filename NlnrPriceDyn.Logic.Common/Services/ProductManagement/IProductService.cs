using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;

namespace NlnrPriceDyn.Logic.Common.Services
{
    public interface IProductService
    {
        Task<UserProduct> CreateUserProductAsync(CreateUpdateProductRequest request);
        Task<UserProduct> GetProductByIdAsync(string id);
        Task UpdateProductPrices();
        Task<IEnumerable<UserProduct>> GetUserProductListAsync(string userId);
        Task<Object> GetUserProductPriceData(string userProductId, DateTime? periodLowestRange);
        Task DeleteProductAsync(string userId, string id);
        Task<ProductNotificationDB> UpdateUserProductNotification(CreateUpdateProductNotificationRequest request);
        Task<UserProductNotification> GetUserProductNotificationByProductId(string id);
    }
}
