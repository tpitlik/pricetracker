using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Services;
using NlnrPriceDyn.Web.Controllers;
using NUnit.Framework;

namespace NlnrPriceDyn.Tests
{
    public class ProductsControllerTests
    {
        public Mock<IProductService> productServiceMock;

        [SetUp]
        public void Setup()
        {
            productServiceMock = new Mock<IProductService>();
        }

        [Test]
        public async Task ProductsControllerAddProductBadRequest()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            productsController.ModelState.AddModelError("usr_id_error", "User id is empty.");

            var result = await productsController.AddProduct(null) as BadRequestObjectResult;

            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerAddProduct()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            var request = new CreateUpdateProductRequest()
            {
                Id = "",
                LinkUrl = "test_url",
                Title = "test_title",
                UserId = "uid-0000-0001"
            };
            var createdUserProduct = new UserProduct()
            {
                Id = Guid.NewGuid().ToString(),
                LinkUrl = request.LinkUrl,
                UserId = request.UserId,
                Title = request.Title
            };

            productServiceMock.Setup(x => x.CreateUserProductAsync(It.IsAny<CreateUpdateProductRequest>()))
                .Returns(Task.FromResult(createdUserProduct));

            productsController.ControllerContext.HttpContext = new DefaultHttpContext();
            productsController.HttpContext.User.Claims.Append(new Claim("id", "uid-0000-0001"));

            var result = await productsController.AddProduct(request) as CreatedResult;

            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerGetProductExistingProduct()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            productServiceMock.Setup(x => x.GetProductByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new UserProduct()));
            var result = await productsController.GetProduct("up0-0000-0001") as OkObjectResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerGetProductNotExistingProduct()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            productServiceMock.Setup(x => x.GetProductByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<UserProduct>(null));
            var result = await productsController.GetProduct("up0-0000-0001") as BadRequestResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerGetUserProductNotificationExistingProduct()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            productServiceMock.Setup(x => x.GetUserProductNotificationByProductId(It.IsAny<string>()))
                .Returns(Task.FromResult(new UserProductNotification()));
            var result = await productsController.GetUserProductNotification("up0-0000-0001") as OkObjectResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerGetUserProductNotificationNotExistingProduct()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            productServiceMock.Setup(x => x.GetUserProductNotificationByProductId(It.IsAny<string>()))
                .Returns(Task.FromResult<UserProductNotification>(null));
            var result = await productsController.GetUserProductNotification("up0-0000-0001") as BadRequestResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerUpdateUserProductNotificationBadRequest()
        {
            var productsController = new ProductsController(productServiceMock.Object);

            var request = new CreateUpdateProductNotificationRequest()
            {
                Id = "up0-0000-001",
                IsActive = true,
                LowPriceLimit = "100",
                TriggerOnce = true
            };

            productsController.ModelState.AddModelError("request_error", "incorrect_request");

            var result = await productsController.UpdateProductNotification(request) as BadRequestResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerUpdateUserProductNotificationNotExistingProductNotification()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            productServiceMock
                .Setup(x => x.UpdateUserProductNotification(It.IsAny<CreateUpdateProductNotificationRequest>()))
                .Returns(Task.FromResult<ProductNotificationDB>(null));

            var request = new CreateUpdateProductNotificationRequest();

            var result = await productsController.UpdateProductNotification(request) as BadRequestResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerUpdateUserProductNotificationExistingProductNotification()
        {
            var request = new CreateUpdateProductNotificationRequest()
            {
                Id = "up0-0000-001",
                IsActive = true,
                LowPriceLimit = "100",
                TriggerOnce = true
            };

            var productNotification = new ProductNotificationDB()
            {
                Id = request.Id
            };

            var productsController = new ProductsController(productServiceMock.Object);

            productServiceMock
                .Setup(x => x.UpdateUserProductNotification(It.IsAny<CreateUpdateProductNotificationRequest>()))
                .Returns(Task.FromResult<ProductNotificationDB>(productNotification));

            var result = await productsController.UpdateProductNotification(request) as OkResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerGetProductPriceDataExistingProduct()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            productServiceMock.Setup(x => x.GetUserProductPriceData(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(
                Task.FromResult(new object()));

            var result = await productsController.GetProductPriceData("up0-0000-001", "week");

            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerGetProductPriceDataNotExistingProduct()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            productServiceMock.Setup(x => x.GetUserProductPriceData(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(
                Task.FromResult<object>(null));

            var result = await productsController.GetProductPriceData("up0-0000-001", "week") as BadRequestResult;

            Assert.NotNull(result);
        }

        [Test]
        public async Task ProductsControllerGetProductList()
        {
            var productsController = new ProductsController(productServiceMock.Object);
            productsController.ControllerContext.HttpContext = new DefaultHttpContext();
            productsController.HttpContext.User.Claims.Append(new Claim("id", "uid-0000-0001"));

            productServiceMock.Setup(x => x.GetUserProductListAsync(It.IsAny<string>())).Returns(
                Task.FromResult<IEnumerable<UserProduct>>(new List<UserProduct>()));

            var result = await productsController.GetProductList() as OkObjectResult;

            Assert.NotNull(result);
        }
    }
}