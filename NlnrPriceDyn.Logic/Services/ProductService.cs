using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.DataAccess.Common.Repositories.ProductManagement;
using NlnrPriceDyn.Logic.Common;
using NlnrPriceDyn.Logic.Common.Models.Messaging;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Services;
using NlnrPriceDyn.Logic.Common.Services.Messaging;

namespace NlnrPriceDyn.Logic.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IUserService _userService;
        private readonly IMailService _mailer;

        private readonly string ApiCatalogueUrl = Resource.ApiCatalogueUrl;
        private readonly string CatalogueHost = Resource.CatalogueHost;

        public ProductService(IProductRepository repository, IMapper mapper, ILogger<ProductService> logger, IUserService userService, IMailService mailer)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _mailer = mailer;
        }


        public async Task<UserProduct> CreateUserProductAsync(CreateUpdateProductRequest request)
        {
            UserProductDB userProductDb = _mapper.Map<UserProductDB>(request);
            var productCatalogueInfo = await GetProductCatalogueId(request.LinkUrl);
            userProductDb.CatalogueName = productCatalogueInfo.Item1;
            userProductDb.CatalogueId = productCatalogueInfo.Item2;
            return await _repository.CreateUserProductAsync(userProductDb)
                .ContinueWith(t => _mapper.Map<UserProduct>(t.Result));
        }

        public Task<UserProduct> GetProductByIdAsync(string id)
        {
            return _repository.GetUserProductAsync(id).ContinueWith(t => _mapper.Map<UserProduct>(t.Result));
        }

        public async Task<IEnumerable<UserProduct>> GetUserProductListAsync(string userId)
        {
            var userProductDbList = await _repository.GetUserProductsAsync(userId);
            var userProductList = _mapper.Map<IEnumerable<UserProduct>>(userProductDbList);
            return userProductList;
        }

        public async Task<Object> GetUserProductPriceData(string userProductId, DateTime? periodLowestRange)
        {
            var userProductPriceInfoDbs = await _repository.GetUserProductPriceListAsync(userProductId, periodLowestRange);

            if (userProductPriceInfoDbs != null)
            {
                List<List<string>> minMaxPriceData = new List<List<string>>();
                List<List<string>> averagePriceData = new List<List<string>>();

                foreach (var entity in userProductPriceInfoDbs)
                {
                    var dateTimeOffset = new DateTimeOffset(entity.TimeStamp);
                    var unixDateTime = (dateTimeOffset.ToUnixTimeSeconds() * 1000).ToString();
                    minMaxPriceData.Add(new List<string>(new string[]
                    {
                        unixDateTime, entity.MinPrice.ToString("#.##", CultureInfo.InvariantCulture),
                        entity.MaxPrice.ToString("#.##", CultureInfo.InvariantCulture)
                    }));
                    averagePriceData.Add(new List<string>(new string[]
                        {unixDateTime, entity.MeanPrice.ToString("#.##", CultureInfo.InvariantCulture)}));
                }

                var jsonObject = new {priceRange = minMaxPriceData, priceAverage = averagePriceData};

                return jsonObject;
            }
            else
            {
                return null;
            }
        }

        public async Task UpdateProductPrices()
        {
            TimeHelper.Update();
            await _repository.UpdateProductPriceInfos();
            await SendProductPriceNotifications();
        }

        public async Task DeleteProductAsync(string userId, string userProductId)
        {
            var userProduct = await _repository.GetUserProductAsync(userProductId);
            if (userProduct.UserId == userId)
            {
                await _repository.DeleteUserProductAsync(userProductId);
            }
            else
            {
                throw new ProductServiceException(Resource.ErrorIncorrectUserProductId);
            }
        }

        public virtual async Task<(string, string)> GetProductCatalogueId(string productUrl)
        {
            char[] charsToTrim = { '/' };
            Uri productUri;
            string productCatalogueId;
            string productName;
            try
            {
                productUri = new Uri(productUrl);
                if (productUri.Host != CatalogueHost)
                {
                    throw new UriFormatException();
                }
                productCatalogueId = productUri.Segments[3].Trim(charsToTrim);
                productName = await CheckProductCatalogueId(productCatalogueId);
            }
            catch (ProductServiceException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ProductServiceException(Resource.ErrorIncorrectProductUrl);
            }

            var result = (productName, productCatalogueId);
            return result;

        }

        private async Task<string> CheckProductCatalogueId(string productId)
        {
            string checkUrl = $"{ApiCatalogueUrl}{productId}";
            HttpClient httpClient = new HttpClient();
            string jsonResult;
            string productName;
            try
            {
                jsonResult = await httpClient.GetStringAsync(checkUrl);
            }
            catch (Exception e)
            {
                throw new ProductServiceException(Resource.ErrorServerError);
            }
            JObject jsonObj = JObject.Parse(jsonResult);
            var jProductsArray = (JArray)jsonObj["products"];
            var jProduct = jProductsArray.FirstOrDefault(p => (string )p["key"] == productId);
            if (jProduct == null)
            {
                throw new ProductServiceException(Resource.ErrorIncorrectProductUrl);
            }
            else
            {
                if ((string)jProduct["status"] == "old")
                {
                    throw new ProductServiceException(Resource.ErrorProductDiscontinued);
                }

                productName = (string) jProduct["name"];
            }
            return productName;
        }

        public Task<ProductNotificationDB> UpdateUserProductNotification(CreateUpdateProductNotificationRequest request)
        {
            ProductNotificationDB userProductNotificationDb = _mapper.Map<ProductNotificationDB>(request);
            return _repository.UpdateUserProductNotificationAsync(userProductNotificationDb);
        }

        public Task<UserProductNotification> GetUserProductNotificationByProductId(string productId)
        {
            return _repository.GetUserProductAsync(productId).ContinueWith(t => _mapper.Map<UserProductNotification>(t.Result));
        }

        public async Task<int> SendProductPriceNotifications()
        {

            var userProductDbList = await _repository.QueryUserProductDbAsync(p =>
                p.ProductNotificationDb.IsActive && p.ProductDb.OffersCount > 0 && p.ProductDb.MinPrice < 1.05*p.ProductNotificationDb.LowPriceLimit);

            int count = 0;

            foreach (var p in userProductDbList)
            {
                UserDB userIdentity = await _userService.FindByIdAsync(p.UserId);
                var message = new EmailMessage()
                {
                    Body = String.Format(Resource.EmailProductLowPriceNotificationBody, p.Title, p.ProductDb.MinPrice),
                    Destination = userIdentity.Email,
                    Subject = Resource.EmailProductLowPriceNotificationSubject
                };
                //_logger.LogWarning(String.Format("Notification info: user id: {0}, product id: {1}, user email {2}", p.UserId, p.Id, userIdentity.Email));
                await _mailer.SendAsync(message);
                //_logger.LogWarning("Notification sent succesfully");

                if (p.ProductNotificationDb.TriggerOnce)
                {
                    await _repository.SetProductNotificationTriggered(p.ProductNotificationDb.Id);
                }

                count++;
            }

            return count;
        }

    }
}
