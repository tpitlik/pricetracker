using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.DataAccess.Contexts;
using NlnrPriceDyn.DataAccess.Repositories;
using System.Linq.Expressions;
using System;
using System.Threading;
using Moq;
using NlnrPriceDyn.DataAccess.Common.Exceptions;

namespace NlnrPriceDyn.Tests
{

    public class ProductRepositoryTests
    {

        public DbContextOptions<ApplicationDbContext> DummyOptions { get; } =
            new DbContextOptionsBuilder<ApplicationDbContext>().Options;

        public DbContextMock<ApplicationDbContext> AppDbContextMock;
        public DbSetMock<ProductDB> ProductDbSetMock;
        public DbSetMock<UserProductDB> UserProductDbSetMock;
        public DbSetMock<ProductNotificationDB> ProductNotificationDbSetMock;
        public DbSetMock<ProductPriceInfoDB> ProductPriceInfoDbSetMock;

        [SetUp]
        public void Setup()
        {
            var productDb1 =
                new ProductDB
                {
                    Id = "p000-0000-001",
                    CatalogueId = "catid1",
                    CatalogueName = "tstproduct1",
                    RefsCount = 1
                };

            var productNotificationDb1 = new ProductNotificationDB
            {
                Id = "pn000-0000-001",
                IsActive = false,
                LowPriceLimit = 0,
                TriggerOnce = false,
                UserId = "u000-0000-001",
            };

            var productNotificationDb2 = new ProductNotificationDB
            {
                Id = "pn000-0000-002",
                IsActive = true,
                LowPriceLimit = 0,
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
                productNotificationDb1,
                productNotificationDb2
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
        }

        [Test]
        public async Task ProductRepositoryCreateUserProductAsyncExistingProduct()
        {

            var productRepository = new ProductRepository(AppDbContextMock.Object);

            var userProductDb = new UserProductDB();
            userProductDb.CatalogueId = "catid1";

            var createdUserProductDb = await productRepository.CreateUserProductAsync(userProductDb);

            Assert.IsNotEmpty(createdUserProductDb.Id);
            Assert.AreEqual(userProductDb.CatalogueId, userProductDb.ProductDb.CatalogueId);
            Assert.AreEqual(2, createdUserProductDb.ProductDb.RefsCount);
            Assert.IsNotNull(createdUserProductDb.ProductNotificationDb);
        }

        [Test]
        public async Task ProductRepositoryCreateUserProductAsyncNotExistingProduct()
        {

            var productRepository = new ProductRepository(AppDbContextMock.Object);

            var userProductDb = new UserProductDB();
            userProductDb.CatalogueId = "catid2";

            var createdUserProductDb = await productRepository.CreateUserProductAsync(userProductDb);

            Assert.IsNotEmpty(createdUserProductDb.Id);
            Assert.AreEqual(userProductDb.CatalogueId, userProductDb.ProductDb.CatalogueId);
            Assert.AreEqual(1, createdUserProductDb.ProductDb.RefsCount);
            Assert.IsNotNull(createdUserProductDb.ProductNotificationDb);
        }

        [Test]
        public async Task ProductRepositoryDeleteUserProductAsync()
        {
            var productRepository = new ProductRepository(AppDbContextMock.Object);

            await productRepository.DeleteUserProductAsync("up000-0000-001");

            var userProductDb = AppDbContextMock.Object.UsersProducts.Where(p => p.Id == "up000-0000-001")
                .FirstOrDefault();
            var productDb = AppDbContextMock.Object.Products.Where(p => p.Id == "p000-0000-001").FirstOrDefault();
            var productNotificationDb = AppDbContextMock.Object.ProductNotifications
                .Where(p => p.Id == "pn000-0000-001").FirstOrDefault();

            Assert.IsNull(userProductDb);
            Assert.IsNull(productNotificationDb);
            Assert.IsNotNull(productDb);
            Assert.AreEqual(0, productDb.RefsCount);
        }

        [Test]
        public async Task ProductRepositoryGetUserProductAsyncExistingUserProduct()
        {
            var productRepository = new ProductRepository(AppDbContextMock.Object);
            string userProductId = "up000-0000-001";

            var result = await productRepository.GetUserProductAsync(userProductId);
            
            Assert.NotNull(result);
            Assert.AreEqual(userProductId, result.Id);

            Assert.NotNull(result.ProductDb);
            Assert.AreEqual("p000-0000-001", result.ProductDb.Id);

            Assert.NotNull(result.ProductNotificationDb);
            Assert.AreEqual("pn000-0000-001", result.ProductNotificationDb.Id);
        }

        [Test]
        public async Task ProductRepositoryGetUserProductAsyncNotExistingUserProduct()
        {
            var productRepository = new ProductRepository(AppDbContextMock.Object);
            string userProductId = "up000-0000-005";

            var result = await productRepository.GetUserProductAsync(userProductId);
            Assert.IsNull(result);
        }

        [Test] public async Task ProductRepositoryGetProductAsyncExistingProduct()
        {
            var productRepository = new ProductRepository(AppDbContextMock.Object);
            string id = "p000-0000-001";
            var product = await productRepository.GetProductAsync(id);
            Assert.NotNull(product);
            Assert.AreEqual(id, product.Id);
        }

        [Test]
        public async Task ProductRepositoryGetProductAsyncNotExistingProduct()
        {
            var productRepository = new ProductRepository(AppDbContextMock.Object);
            string id = "p000-0000-002";
            var product = await productRepository.GetProductAsync(id);
            Assert.IsNull(product);
        }

        [Test]
        public async Task ProductRepositoryQueryUserProductDbAsync()
        {
            var productRepository = new ProductRepository(AppDbContextMock.Object);
            string userId = "uid000-0000-001";
            var result = await productRepository.QueryUserProductDbAsync(FilterByUserId(userId));
            var userProduct = result.FirstOrDefault<UserProductDB>();

            Assert.NotNull(userProduct);
            Assert.AreEqual(userId, userProduct.UserId);
            Assert.NotNull(userProduct.ProductDb);
            Assert.NotNull(userProduct.ProductNotificationDb);
        }

        public Expression<Func<UserProductDB, bool>> FilterByUserId(string userId)
        {
            return x => x.UserId == userId;
        }

        [Test]
        public async Task ProductRepositoryGetUserProductPriceListAsyncExistingProductWithoutPeriodLowestRange() {
            var productRepository = new ProductRepository(AppDbContextMock.Object);
            var result = await productRepository.GetUserProductPriceListAsync("up000-0000-001", null);
            Assert.AreEqual(4, result.Count());
        }

        [Test]
        public async Task ProductRepositoryGetUserProductPriceListAsyncExistingProductWithPeriodLowestRange()
        {
            var productRepository = new ProductRepository(AppDbContextMock.Object);
            var result = await productRepository.GetUserProductPriceListAsync("up000-0000-001", DateTime.UtcNow.AddDays(-10));
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task ProductRepositoryGetUserProductPriceListAsyncNotExistingProduct()
        {
            var productRepository = new ProductRepository(AppDbContextMock.Object);
            var result = await productRepository.GetUserProductPriceListAsync("up000-0000-002", null);
            Assert.IsNull(result);
        }

        [Test]
        public async Task ProductRepositoryUpdateUserProductNotificationAsync() {
            var productRepository = new ProductRepository(AppDbContextMock.Object);
            var productNotificationDb1 = new ProductNotificationDB
            {
                Id = "pn000-0000-001",
                IsActive = true,
                LowPriceLimit = 100,
                TriggerOnce = true,
                UserId = "u000-0000-001",
            };
            var result = await productRepository.UpdateUserProductNotificationAsync(productNotificationDb1);
            Assert.AreEqual(productNotificationDb1.IsActive, result.IsActive);
            Assert.AreEqual(productNotificationDb1.LowPriceLimit, result.LowPriceLimit);
            Assert.AreEqual(productNotificationDb1.LowPriceLimit, result.LowPriceLimit);
            Assert.AreEqual(productNotificationDb1.UserId, result.UserId);
        }

        [Test]
        public async Task ProductRepositoryUpdateProductPriceInfoExistingProductInStock()
        {
            var productDb1 = new ProductDB
            {
                Id = "p000-0000-001"
            };

            var prevProductPriceInfoDb = new ProductPriceInfoDB
            {
                Id = 4,
                TimeStamp = DateTime.UtcNow.AddMinutes(-30),
                MinPrice = 10,
                MeanPrice = 20,
                MaxPrice = 30,
                OffersCount = 2,
                Product = productDb1
            };

            var newProductPriceInfoDb = new ProductPriceInfoDB
            {
                Id = 5,
                TimeStamp = DateTime.UtcNow,
                MinPrice = 15,
                MeanPrice = 30,
                MaxPrice = 45,
                OffersCount = 4,
                Product = productDb1
            };

            var productRepositoryMock = new Mock<ProductRepository>(AppDbContextMock.Object);
            productRepositoryMock.Setup(x => x.CreateProductPriceInfoDbEntity(It.IsAny<string>())).Returns(Task.FromResult<ProductPriceInfoDB>(newProductPriceInfoDb));
            productRepositoryMock.Setup(x => x.GetPrevProductPriceInfo(It.IsAny<string>())).Returns(prevProductPriceInfoDb);
            var result = await productRepositoryMock.Object.UpdateProductPriceInfo("p000-0000-001");

            Assert.NotNull(result);
            Assert.AreEqual(newProductPriceInfoDb.MinPrice, result.MinPrice);
            Assert.AreEqual(newProductPriceInfoDb.MeanPrice, result.MeanPrice);
            Assert.AreEqual(newProductPriceInfoDb.MaxPrice, result.MaxPrice);
            Assert.AreEqual(newProductPriceInfoDb.OffersCount, result.OffersCount);
            Assert.AreEqual(newProductPriceInfoDb.MinPrice - prevProductPriceInfoDb.MinPrice, result.MinPriceChange);
            Assert.AreEqual(newProductPriceInfoDb.MeanPrice - prevProductPriceInfoDb.MeanPrice, result.MeanPriceChange);
            Assert.AreEqual(newProductPriceInfoDb.MaxPrice - prevProductPriceInfoDb.MaxPrice, result.MaxPriceChange);
        }

        [Test]
        public async Task ProductRepositoryUpdateProductPriceInfoExistingProductOutOfStock()
        {
            var productDb1 = new ProductDB
            {
                Id = "p000-0000-001"
            };

            var prevProductPriceInfoDb = new ProductPriceInfoDB
            {
                Id = 4,
                TimeStamp = DateTime.UtcNow.AddMinutes(-30),
                MinPrice = 10,
                MeanPrice = 20,
                MaxPrice = 30,
                OffersCount = 2,
                Product = productDb1
            };

            var productRepositoryMock = new Mock<ProductRepository>(AppDbContextMock.Object);
            productRepositoryMock.Setup(x => x.CreateProductPriceInfoDbEntity(It.IsAny<string>())).Returns(Task.FromResult<ProductPriceInfoDB>(null));
            productRepositoryMock.Setup(x => x.GetPrevProductPriceInfo(It.IsAny<string>())).Returns(prevProductPriceInfoDb);
            var result = await productRepositoryMock.Object.UpdateProductPriceInfo("p000-0000-001");

            Assert.NotNull(result);
            Assert.AreEqual(0, result.MinPrice);
            Assert.AreEqual(0, result.MeanPrice);
            Assert.AreEqual(0, result.MaxPrice);
            Assert.AreEqual(0, result.MinPriceChange);
            Assert.AreEqual(0, result.MeanPriceChange);
            Assert.AreEqual(0, result.MaxPriceChange);
            Assert.AreEqual(0, result.OffersCount);
        }

        [Test]
        public async Task ProductRepositoryUpdateProductPriceInfoNewProduct()
        {
            var productDb1 = new ProductDB
            {
                Id = "p000-0000-001"
            };

            var newProductPriceInfoDb = new ProductPriceInfoDB
            {
                Id = 5,
                TimeStamp = DateTime.UtcNow,
                MinPrice = 15,
                MeanPrice = 30,
                MaxPrice = 45,
                OffersCount = 4,
                Product = productDb1
            };

            var productRepositoryMock = new Mock<ProductRepository>(AppDbContextMock.Object);
            productRepositoryMock.Setup(x => x.CreateProductPriceInfoDbEntity(It.IsAny<string>())).Returns(Task.FromResult<ProductPriceInfoDB>(newProductPriceInfoDb));
            productRepositoryMock.Setup(x => x.GetPrevProductPriceInfo(It.IsAny<string>())).Returns<ProductPriceInfoDB>(null);
            var result = await productRepositoryMock.Object.UpdateProductPriceInfo("p000-0000-001", true);

            Assert.NotNull(result);
            Assert.AreEqual(newProductPriceInfoDb.MinPrice, result.MinPrice);
            Assert.AreEqual(newProductPriceInfoDb.MeanPrice, result.MeanPrice);
            Assert.AreEqual(newProductPriceInfoDb.MaxPrice, result.MaxPrice);
            Assert.AreEqual(0, result.MinPriceChange);
            Assert.AreEqual(0, result.MeanPriceChange);
            Assert.AreEqual(0, result.MaxPriceChange);
            Assert.AreEqual(newProductPriceInfoDb.OffersCount, result.OffersCount);
        }

        [Test]
        public async Task ProductRepositorySetProductNotificationTriggeredExistingNotification()
        {
            var productRepositoryMock = new Mock<ProductRepository>(AppDbContextMock.Object);
            await productRepositoryMock.Object.SetProductNotificationTriggered("pn000-0000-002");
            AppDbContextMock.Verify(x=>x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ProductRepositorySetProductNotificationTriggeredNotExistingNotification()
        {
            var productRepositoryMock = new Mock<ProductRepository>(AppDbContextMock.Object);

            Assert.ThrowsAsync<ProductRepositoryException>(() =>
                productRepositoryMock.Object.SetProductNotificationTriggered("pn000-0000-005"));
        }
    }
}