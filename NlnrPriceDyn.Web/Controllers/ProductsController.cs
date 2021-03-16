using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NlnrPriceDyn.Logic.Common;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;
using NlnrPriceDyn.Logic.Common.Services;
using NlnrPriceDyn.Web.Helpers;

namespace NlnrPriceDyn.Web.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    //[Authorize(Policy = "ApiUser")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // POST api/products/add
        [Authorize(Policy = Constants.Strings.ApiPolicies.GeneralUserPolicy)]
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] CreateUpdateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            request.UserId = this.HttpContext.User.Claims.Where(c => c.Type == "id").Select(c => c.Value).SingleOrDefault();
            var product = await _productService.CreateUserProductAsync(request);
            var location = $"/api/Products/{product.Id}";
            return Created(location, product);
        }

        // GET api/products/notification/id
        [Authorize(Policy = Constants.Strings.ApiPolicies.DemoGeneralUserPolicy)]
        [HttpGet]
        [Route("{id}/notification")]
        public async Task<IActionResult> GetUserProductNotification(string id)
        {
            var userProductNotification = await _productService.GetUserProductNotificationByProductId(id);
            if (userProductNotification != null)
            {
                return Ok(userProductNotification);
            }
            else
            {
                return BadRequest();
            }
        }

        // POST api/products/notification/update
        [Authorize(Policy = Constants.Strings.ApiPolicies.DemoGeneralUserPolicy)]
        [HttpPost("notification/update")]
        public async Task<IActionResult> UpdateProductNotification([FromBody] CreateUpdateProductNotificationRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.UpdateUserProductNotification(request);
                if (result != null)
                {
                    return Ok();
                }
            };

            return BadRequest();
        }

        // GET api/products/id
        [Authorize(Policy = Constants.Strings.ApiPolicies.AdminUserPolicy)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product != null)
            {
                return Ok(product);
            }
            else
            {
                return BadRequest();
            }
        }

        // GET api/products/price/{id}?period={period}
        [Authorize(Policy = Constants.Strings.ApiPolicies.DemoGeneralUserPolicy)]
        [HttpGet]
        [Route("price/{id}")]
        public async Task<IActionResult> GetProductPriceData(string id, [FromQuery] string period)
        {
            DateTime currentTime = DateTime.UtcNow;
            DateTime? periodLowestRange;
            switch (period)
            {
                case "day":
                    periodLowestRange = currentTime.AddHours(-24);
                    break;
                case "week":
                    periodLowestRange = currentTime.AddDays(-7);
                    break;
                case "month":
                    periodLowestRange = currentTime.AddMonths(-1);
                    break;
                case "halfyear":
                    periodLowestRange = currentTime.AddMonths(-6);
                    break;
                case "year":
                    periodLowestRange = currentTime.AddYears(-1);
                    break;
                case "all":
                    periodLowestRange = null;
                    break;
                default:
                    periodLowestRange = currentTime.AddDays(-7);
                    break;
            }
            var jsonResult = await _productService.GetUserProductPriceData(id, periodLowestRange);
            if (jsonResult != null)
            {
                return Ok(jsonResult);
            }
            else
            {
                return BadRequest();
            }
        }

        // GET api/products/list
        [Authorize(Policy = Constants.Strings.ApiPolicies.DemoGeneralUserPolicy)]
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetProductList()
        {
            string userId = this.HttpContext.User.Claims.Where(c => c.Type == "id").Select(c => c.Value).SingleOrDefault();
            //string userId = "f9b5b79a-9ad6-4910-bbe9-cdb91da878d9";
            var products = await _productService.GetUserProductListAsync(userId);
            return Ok(products);
        }

        // GET api/products/update
        [Authorize(Policy = Constants.Strings.ApiPolicies.AdminUserPolicy)]
        [HttpGet]
        [Route("update")]
        public async Task<IActionResult> UpdateProductPrices()
        {
            await _productService.UpdateProductPrices();
            return Ok();
        }

        // GET api/products/delete
        [Authorize(Policy = Constants.Strings.ApiPolicies.GeneralUserPolicy)]
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            string userId = this.HttpContext.User.Claims.Where(c => c.Type == "id").Select(c => c.Value).SingleOrDefault();
            await _productService.DeleteProductAsync(userId, id);
            return Ok();
        }

        // GET api/products/update/last
        [HttpGet]
        [Route("update/last")]
        [AllowAnonymous]
        public IActionResult GetLastUpdateTime()
        {
            var jsonObject = new {timestamp = TimeHelper.LastUpdateSecondsCount};
            return Ok(jsonObject);
        }
    }
}