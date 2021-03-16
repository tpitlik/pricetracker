using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NlnrPriceDyn.DataAccess.Common.Exceptions;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.DataAccess.Common.Repositories.ProductManagement;
using NlnrPriceDyn.DataAccess.Contexts;

namespace NlnrPriceDyn.DataAccess.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private const int UtcToMinskTimezoneOffset = 3;
        private readonly ApplicationDbContext _context;
        private DateTime currentTimestamp { get; set; }

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserProductDB> CreateUserProductAsync(UserProductDB userProductDb)
        {
            currentTimestamp = DateTime.UtcNow;
            userProductDb.Id = Guid.NewGuid().ToString();
            //userProductDb.Added = DateTime.UtcNow;
            userProductDb.Added = currentTimestamp;
            var productDb = _context.Products.FirstOrDefault(p => p.CatalogueId == userProductDb.CatalogueId);
            if (productDb == null)
            {
                productDb = new ProductDB()
                {
                    Id = Guid.NewGuid().ToString(),
                    //Added = DateTime.UtcNow,
                    Added = currentTimestamp,
                    PriceList = new List<ProductPriceInfoDB>(),
                    CatalogueId = userProductDb.CatalogueId,
                    CatalogueName = userProductDb.CatalogueName,
                    OutOfStock = true,
                    RefsCount = 1
                };
                _context.Products.Add(productDb);
                await _context.SaveChangesAsync();
                await UpdateProductPriceInfo(productDb.Id, true);
            }
            else
            {
                if (productDb.RefsCount == 0)
                {
                    await UpdateProductPriceInfo(productDb.Id);
                }
                productDb.RefsCount++;
            }
            userProductDb.ProductDb = productDb;
            
            //Create default user product price notification
            ProductNotificationDB productNotificationDB = await CreateDefaultUserProductNotificationAsync(userProductDb);
            userProductDb.ProductNotificationDb = productNotificationDB;

            //Save userProductDb
            _context.UsersProducts.Add(userProductDb);
            await _context.SaveChangesAsync();

            return userProductDb;
        }

        public async Task<IEnumerable<ProductPriceInfoDB>> GetUserProductPriceListAsync(string userProductId, DateTime? periodLowestRange)
        {

            IEnumerable<ProductPriceInfoDB> productPriceOverPeriod;
            var userProductDb = await _context.UsersProducts.AsNoTracking().Where(p => p.Id == userProductId).Include(p => p.ProductDb).FirstOrDefaultAsync();

            if (userProductDb != null)
            {
                if (periodLowestRange != null)
                {
                    productPriceOverPeriod = _context.ProductPriceInfos.AsNoTracking().Include(p => p.Product).Where(p => p.Product.Id == userProductDb.ProductDb.Id && p.TimeStamp >= periodLowestRange).ToList();
                }
                else
                {
                    productPriceOverPeriod = _context.ProductPriceInfos.AsNoTracking().Include(p => p.Product).Where(p => p.Product.Id == userProductDb.ProductDb.Id).ToList();
                }

                return productPriceOverPeriod.OrderBy(p => p.TimeStamp);
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<UserProductDB>> GetUserProductsAsync(string userId)
        {
            return await _context.UsersProducts.Where(p => p.UserId == userId).AsNoTracking().Include(p=>p.ProductDb).Include(p=>p.ProductNotificationDb).OrderByDescending(p => p.Added).ToListAsync();

        }

        public Task<ProductDB> GetProductAsync(string productId)
        {
            return _context.Products.Where(p => p.Id == productId).Include(p => p.PriceList).FirstOrDefaultAsync();
        }

        public Task<UserProductDB> GetUserProductAsync(string userProductId)
        {
            return _context.UsersProducts.Where(p => p.Id == userProductId).Include(p => p.ProductDb).Include(p=>p.ProductNotificationDb).FirstOrDefaultAsync();
        }

        public async Task<ProductDB> UpdateProductPriceInfo(string productId, bool newProduct = false)
        {
            var productDb = _context.Products.FirstOrDefault(p => p.Id == productId);
            var productPriceInfoDb = await CreateProductPriceInfoDbEntity(productDb.CatalogueId);
            if (productPriceInfoDb!=null)
            {
                productPriceInfoDb.Product = productDb;
                _context.ProductPriceInfos.Add(productPriceInfoDb);
                await _context.SaveChangesAsync();

                if (!newProduct)
                {
                    var productPriceOverPeriod = GetPrevProductPriceInfo(productId);

                    if (productPriceOverPeriod != null)
                    {
                        productDb.MinPriceChange = productPriceInfoDb.MinPrice - productPriceOverPeriod.MinPrice;
                        productDb.MaxPriceChange = productPriceInfoDb.MaxPrice - productPriceOverPeriod.MaxPrice;
                        productDb.MeanPriceChange = productPriceInfoDb.MeanPrice - productPriceOverPeriod.MeanPrice;
                    }
                }
                productDb.MinPrice = productPriceInfoDb.MinPrice;
                productDb.MaxPrice = productPriceInfoDb.MaxPrice;
                productDb.MeanPrice = productPriceInfoDb.MeanPrice;
                productDb.OffersCount = productPriceInfoDb.OffersCount;
                productDb.OutOfStock = false;
            }
            else
            {
                productDb.OutOfStock = true;
                productDb.OffersCount = 0;
                productDb.MinPrice = 0;
                productDb.MaxPrice = 0;
                productDb.MeanPrice = 0;
                productDb.MinPriceChange = 0;
                productDb.MaxPriceChange = 0;
                productDb.MeanPriceChange = 0;
            }

            await _context.SaveChangesAsync();
            return productDb;
        }

        public async Task UpdateProductPriceInfos()
        {
            //var productDbs = _context.Products.Include(p => p.PriceList).Where(p => p.RefsCount > 0).ToList();
            currentTimestamp = DateTime.UtcNow;
            var productDbs = _context.Products.AsNoTracking().Where(p => p.RefsCount > 0).ToList();
            foreach (var productDb in productDbs)
            {
                await UpdateProductPriceInfo(productDb.Id);

            }
            //await _context.SaveChangesAsync();
        }

        public virtual Task DeleteUserProductAsync(string userProductId)
        {
            //UserProductDB userProductDb = new UserProductDB() {Id = userProductId};
            //_context.UsersProducts.Remove(userProductDb);
            var userProductDb = _context.UsersProducts.Include(p => p.ProductDb).Include(p=>p.ProductNotificationDb).Where(p => p.Id == userProductId)
                .SingleOrDefault();
            var productDb = _context.Products.Find(userProductDb.ProductDb.Id);
            productDb.RefsCount--;
            _context.ProductNotifications.Remove(userProductDb.ProductNotificationDb);
            _context.UsersProducts.Remove(userProductDb);
            return _context.SaveChangesAsync();
        }

        public virtual async Task<ProductPriceInfoDB> CreateProductPriceInfoDbEntity(string catalogueId)
        {
            IEnumerable<float> productPriceList = new List<float>();
            try
            {
                productPriceList = await GetProductPriceFromCatalogueAsync(catalogueId);
            }
            catch (ProductRepositoryException e)
            {
                return null;
            }
            catch (Exception e)
            {
                throw new ProductRepositoryException(e.Message);
            }
            ProductPriceInfoDB productPriceInfoDb = new ProductPriceInfoDB();
            productPriceInfoDb.MinPrice = productPriceList.Min();
            productPriceInfoDb.MaxPrice = productPriceList.Max();
            productPriceInfoDb.MeanPrice = (float)Math.Round(productPriceList.Average(), 2, MidpointRounding.AwayFromZero);
            productPriceInfoDb.OffersCount = productPriceList.Count();
            //productPriceInfoDb.TimeStamp = DateTime.UtcNow;
            productPriceInfoDb.TimeStamp = currentTimestamp;
            return productPriceInfoDb;
        }

        private async Task<IEnumerable<float>> GetProductPriceFromCatalogueAsync(string productCatalogueId)
        {
            string apiUrl = String.Format(Resource.ShopApiUrl, productCatalogueId);

            HttpClient httpClient = new HttpClient();
            string jsonResult;
            try
            {
                jsonResult = await httpClient.GetStringAsync(apiUrl);
            }
            catch (Exception e)
            {
                throw new ProductRepositoryException(e.Message);
            }
            JObject jsonObj = JObject.Parse(jsonResult);
            JArray positions = (JArray)jsonObj["positions"]["primary"];
            IEnumerable<float> productPriceList = positions.Select(p => float.Parse((string)p["position_price"]["amount"], CultureInfo.InvariantCulture));
            return productPriceList;
        }

        public async Task<ProductNotificationDB> CreateDefaultUserProductNotificationAsync(UserProductDB userProductDb)
        {
            ProductNotificationDB userProductNotificationDB = new ProductNotificationDB()
            {
                Id = Guid.NewGuid().ToString(),
                IsActive = false,
                TriggerOnce = true,
                LowPriceLimit = 0.0F,
                UserId = userProductDb.UserId
            };
            _context.ProductNotifications.Add(userProductNotificationDB);
            await _context.SaveChangesAsync();
            return userProductNotificationDB;
        }

        public async Task<ProductNotificationDB> UpdateUserProductNotificationAsync(ProductNotificationDB userProductNotificationDbUpdateInfo)
        {
            //var productDb = _context.Products.Find(userProductDb.ProductDb.Id);
            var userProductNotificationDB = _context.ProductNotifications.Find(userProductNotificationDbUpdateInfo.Id);
            if (userProductNotificationDB != null)
            {
                userProductNotificationDB.IsActive = userProductNotificationDbUpdateInfo.IsActive;
                userProductNotificationDB.LowPriceLimit = userProductNotificationDbUpdateInfo.LowPriceLimit;
                userProductNotificationDB.TriggerOnce = userProductNotificationDbUpdateInfo.TriggerOnce;
            }
            else
            {
                throw new ProductRepositoryException(Resource.ErrorIncorrectProductNotificationId);
            }
            await _context.SaveChangesAsync();
            return userProductNotificationDB;
        }

        public async Task<IReadOnlyList<UserProductDB>> QueryUserProductDbAsync(Expression<Func<UserProductDB, bool>> predicate)
        {

            return await _context.UsersProducts.AsNoTracking().Include(p => p.ProductDb)
                .Include(p => p.ProductNotificationDb).Where(predicate).ToListAsync();
        }

        public Task SetProductNotificationTriggered(string productNotificationId)
        {
            var productNotificationDb = _context.ProductNotifications.Find(productNotificationId);
            if (productNotificationDb != null)
            {
                productNotificationDb.IsActive = false;
                return _context.SaveChangesAsync();
            }
            else
            {
                throw new ProductRepositoryException(Resource.ErrorIncorrectProductNotificationId);
            }
        }

        public virtual ProductPriceInfoDB GetPrevProductPriceInfo(string productId)
        {
            var currentDateTime = currentTimestamp.AddHours(UtcToMinskTimezoneOffset);
            var periodLowestRange = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0)
                .AddHours(-1*UtcToMinskTimezoneOffset);
            var productPriceOverPeriod = _context.ProductPriceInfos.AsNoTracking().Include(p => p.Product)
                .Where(p => p.Product.Id == productId && p.TimeStamp >= periodLowestRange).OrderBy(p => p.TimeStamp)
                .FirstOrDefault();

            return productPriceOverPeriod ?? _context.ProductPriceInfos.AsNoTracking().Include(p => p.Product)
                       .Where(p => p.Product.Id == productId && p.TimeStamp < periodLowestRange)
                       .OrderByDescending(p => p.TimeStamp)
                       .FirstOrDefault();

        }
    }
}
