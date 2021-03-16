using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;

namespace NlnrPriceDyn.DataAccess.Common.Repositories.ProductManagement
{
    public interface IProductRepository
    {
        Task<UserProductDB> CreateUserProductAsync(UserProductDB userProductDb);
        Task UpdateProductPriceInfos();
        Task<UserProductDB> GetUserProductAsync(string userProductId);
        Task<IEnumerable<ProductPriceInfoDB>> GetUserProductPriceListAsync(string userProductId, DateTime? periodLowestRange);
        Task<IEnumerable<UserProductDB>> GetUserProductsAsync(string userId);
        Task DeleteUserProductAsync(string userProductId);
        Task<ProductNotificationDB> UpdateUserProductNotificationAsync(ProductNotificationDB userProductNotificationDbUpdateInfo);
        Task<IReadOnlyList<UserProductDB>> QueryUserProductDbAsync(Expression<Func<UserProductDB, bool>> predicate);
        Task SetProductNotificationTriggered(string productNotificationId);
    }
}
