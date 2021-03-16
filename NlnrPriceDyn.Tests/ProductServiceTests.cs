using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.DataAccess.Contexts;
using NlnrPriceDyn.DataAccess.Repositories;
using NlnrPriceDyn.Logic.Services;
using NlnrPriceDyn.Logic.Common.Mappings;
using NUnit.Framework;
using AutoMapper;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.DataAccess.Common.Repositories.ProductManagement;
using NlnrPriceDyn.Logic.Common.Models.Messaging;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Services;
using NlnrPriceDyn.Logic.Common.Services.Messaging;
using NlnrPriceDyn.Logic.Common;

namespace NlnrPriceDyn.Tests
{
    class ProductServiceTests
    {

        public DbContextOptions<ApplicationDbContext> DummyOptions { get; } =
            new DbContextOptionsBuilder<ApplicationDbContext>().Options;

        public DbContextMock<ApplicationDbContext> AppDbContextMock;
        public DbSetMock<ProductDB> ProductDbSetMock;
        public DbSetMock<UserProductDB> UserProductDbSetMock;
        public DbSetMock<ProductNotificationDB> ProductNotificationDbSetMock;
        public DbSetMock<ProductPriceInfoDB> ProductPriceInfoDbSetMock;

        public IProductRepository productRepository;
        public IMapper mapper;

        public UserDB userDb1;

        [SetUp]
        public void Setup()
        {

            userDb1 = new UserDB()
            {
                Id = "uid000-0000-001",
                Email = "test_user@mail.com"
            };

            var productDb1 = new ProductDB
            {
                Id = "p000-0000-001",
                CatalogueId = "catid1",
                CatalogueName = "tstproduct1",
                RefsCount = 1,
                MinPrice = 100.0f,
                MeanPrice = 150.0f,
                MaxPrice = 200.0f,
                OffersCount = 5
            };

            var productNotificationDb1 = new ProductNotificationDB
            {
                Id = "pn000-0000-001",
                IsActive = true,
                LowPriceLimit = 99.0f,
                TriggerOnce = true,
                UserId = "u000-0000-001",
            };

            var userProductDb1 = new UserProductDB
            {
                Id = "up000-0000-001",
                CatalogueId = "catid1",
                CatalogueName = "tstproduct1",
                ProductDb = productDb1,
                ProductNotificationDb = productNotificationDb1,
                UserId = "uid000-0000-001"
            };

            var productPriceInfoDb1 = new ProductPriceInfoDB
            {
                Id = 1,
                TimeStamp = DateTime.UtcNow,
                Product = productDb1
            };

            var productPriceInfoDb2 = new ProductPriceInfoDB
            {
                Id = 2,
                TimeStamp = DateTime.UtcNow.AddDays(-8),
                Product = productDb1
            };

            var productPriceInfoDb3 = new ProductPriceInfoDB
            {
                Id = 3,
                TimeStamp = DateTime.UtcNow.AddDays(-35),
                Product = productDb1
            };

            var productPriceInfoDb4 = new ProductPriceInfoDB
            {
                Id = 4,
                TimeStamp = DateTime.UtcNow.AddDays(-190),
                Product = productDb1
            };

            var productData = new List<ProductDB>
            {
                productDb1
            }.AsQueryable();

            var userProductData = new List<UserProductDB>
            {
                userProductDb1
            }.AsQueryable();

            var productNotificationData = new List<ProductNotificationDB>
            {
                productNotificationDb1
            }.AsQueryable();

            var productPriceInfoData = new List<ProductPriceInfoDB>
            {
                productPriceInfoDb1, productPriceInfoDb2, productPriceInfoDb3, productPriceInfoDb4
            }.AsQueryable();

            AppDbContextMock = new DbContextMock<ApplicationDbContext>(DummyOptions);
            ProductDbSetMock = AppDbContextMock.CreateDbSetMock(x => x.Products, productData);
            UserProductDbSetMock = AppDbContextMock.CreateDbSetMock(x => x.UsersProducts, userProductData);
            ProductNotificationDbSetMock = AppDbContextMock.CreateDbSetMock(x => x.ProductNotifications, productNotificationData);
            ProductPriceInfoDbSetMock = AppDbContextMock.CreateDbSetMock(x => x.ProductPriceInfos, productPriceInfoData);

            productRepository = new ProductRepository(AppDbContextMock.Object);

            var profileCreateUpdateProductRequestToUserProductDb = new CreateUpdateProductRequestToUserProductDb();
            var profileUserProductDbToUserProduct = new UserProductDbToUserProduct();
            var profileUserProductDbToUserProductNotification = new UserProductDbToUserProductNotification();
            var profileCreateUpdateProductNotificationRequestToProductNotificationDb =
                new CreateUpdateProductNotificationRequestToProductNotificationDb();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(profileCreateUpdateProductRequestToUserProductDb);
                cfg.AddProfile(profileUserProductDbToUserProduct);
                cfg.AddProfile(profileUserProductDbToUserProductNotification);
                cfg.AddProfile(profileCreateUpdateProductNotificationRequestToProductNotificationDb);
            });
            mapper = new Mapper(configuration);
        }

        [Test]
        public async Task ProductServiceCreateUserProductAsync()
        {

            var userServiceMock = new Mock<IUserService>();
            var productServiceMock = new Mock<ProductService>(productRepository, mapper, null, userServiceMock.Object, null);
            productServiceMock.Setup(x=>x.GetProductCatalogueId(It.IsAny<string>())).Returns(Task.FromResult(("product_name1", "product_catalogue_id1")));

            var request = new CreateUpdateProductRequest()
            {
                Id = "",
                LinkUrl = "link_url",
                Title = "test_title",
                UserId = "uid000-0000-001"
            };
            var result = await productServiceMock.Object.CreateUserProductAsync(request);

            Assert.NotNull(result);
            Assert.AreNotEqual("",result.Id);
            Assert.AreEqual(request.LinkUrl, result.LinkUrl);
            Assert.AreEqual(request.Title, result.Title);
            Assert.AreEqual(true, result.OutOfStock);
            Assert.AreEqual(0, result.OffersCount);
            Assert.AreNotEqual("", result.ProductNotificationId);
            Assert.AreEqual(request.UserId, result.UserId);
        }

        [Test]
        public async Task ProductServiceDeleteUserProductAsyncCorrectUserId()
        {
            var productRepositoryMock = new Mock<ProductRepository>(AppDbContextMock.Object);
            var productServiceMock = new Mock<ProductService>(productRepositoryMock.Object, null, null, null, null);

            await productServiceMock.Object.DeleteProductAsync("uid000-0000-001", "up000-0000-001");

            productRepositoryMock.Verify(x => x.DeleteUserProductAsync("up000-0000-001"), Times.Once());
        }

        [Test]
        public void ProductServiceDeleteUserProductAsyncIncorrectUserId()
        {
            var productRepositoryMock = new Mock<ProductRepository>(AppDbContextMock.Object);
            var productServiceMock = new Mock<ProductService>(productRepositoryMock.Object, null, null, null, null);

            Assert.ThrowsAsync<ProductServiceException>(() => productServiceMock.Object.DeleteProductAsync("uid000-0000-002", "up000-0000-001"));
        }

        [Test]
        public async Task ProductServiceGetProductByIdAsyncExistingProduct()
        {
            var productService = new ProductService(productRepository, mapper, null, null, null);
            string id = "up000-0000-001";
            var result = await productService.GetProductByIdAsync(id);
            Assert.NotNull(result);
            Assert.AreEqual(id, result.Id);
        }

        [Test]
        public async Task ProductServiceGetProductByIdAsyncNotExistingProduct()
        {
            var productService = new ProductService(productRepository, mapper, null, null, null);
            string id = "up000-0000-005";
            var result = await productService.GetProductByIdAsync(id);
            Assert.Null(result);
        }

        [Test]
        public async Task ProductServiceGetUserProductListAsyncExistingUser()
        {
            var productService = new ProductService(productRepository, mapper, null, null, null);
            string userId = "uid000-0000-001";
            var result = await productService.GetUserProductListAsync(userId);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task ProductServiceGetUserProductListAsyncNotExistingUser()
        {
            var productService = new ProductService(productRepository, mapper, null, null, null);
            string userId = "uid000-0000-005";
            var result = await productService.GetUserProductListAsync(userId);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task ProductServiceGetUserProductPriceDataPeriodAll()
        {
            var productService = new ProductService(productRepository, mapper, null, null, null);
            string id = "up000-0000-001";
            dynamic result = await productService.GetUserProductPriceData(id, null);
            var priceRangeDataList = (List<List<string>>)result?.GetType().GetProperty("priceRange")?.GetValue(result, null);
            var priceAverageDataList = (List<List<string>>)result?.GetType().GetProperty("priceAverage")?.GetValue(result, null);
            Assert.AreEqual(4, priceRangeDataList.Count());
            Assert.AreEqual(4, priceAverageDataList.Count());
        }
        [Test]
        public async Task ProductServiceGetUserProductPriceDataPeriodDaily()
        {
            var productService = new ProductService(productRepository, mapper, null, null, null);
            string id = "up000-0000-001";
            dynamic result = await productService.GetUserProductPriceData(id, DateTime.UtcNow.AddDays(-1));
            var priceRangeDataList = (List<List<string>>)result?.GetType().GetProperty("priceRange")?.GetValue(result, null);
            var priceAverageDataList = (List<List<string>>)result?.GetType().GetProperty("priceAverage")?.GetValue(result, null);
            Assert.AreEqual(1, priceRangeDataList.Count());
            Assert.AreEqual(1, priceAverageDataList.Count());
        }

        [Test]
        public async Task ProductServiceTaskGetUserProductNotificationByProductIdExistingProduct()
        {
            var productService = new ProductService(productRepository, mapper, null, null, null);
            string id = "up000-0000-001";
            var result = await productService.GetUserProductNotificationByProductId(id);
            Assert.AreEqual("pn000-0000-001", result.Id);
        }

        [Test]
        public async Task ProductServiceTaskGetUserProductNotificationByProductIdNotExistingProduct()
        {
            var productService = new ProductService(productRepository, mapper, null, null, null);
            string id = "up000-0000-005";
            var result = await productService.GetUserProductNotificationByProductId(id);
            Assert.Null(result);
        }

        [Test]
        public async Task ProductServiceUpdateUserProductNotification()
        {
            var productService = new ProductService(productRepository, mapper, null, null, null);
            string id = "pn000-0000-001";
            var request = new CreateUpdateProductNotificationRequest()
            {
                Id = id,
                IsActive = true,
                LowPriceLimit = "100.0",
                TriggerOnce = true
            };
            var result = await productService.UpdateUserProductNotification(request);
            Assert.NotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(100.0,result.LowPriceLimit);
            Assert.AreEqual(true, result.IsActive);
            Assert.AreEqual(true, result.TriggerOnce);
        }

        [Test]
        public async Task ProductServiceSendProductPriceNotifications()
        {
            var userServiceMock = new Mock<IUserService>();
            var mailerMock = new Mock<IMailService>();
            mailerMock.Setup(x => x.SendAsync(It.IsAny<EmailMessage>())).Returns(Task.FromResult(true));
            userServiceMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(userDb1));
            var productService = new ProductService(productRepository, mapper, null, userServiceMock.Object, mailerMock.Object);

            var result = await productService.SendProductPriceNotifications();

            Assert.AreEqual(1, result);
            mailerMock.Verify(x=>x.SendAsync(It.IsAny<EmailMessage>()), Times.Once);
        }
    }
}
