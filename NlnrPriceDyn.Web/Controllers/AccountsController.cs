using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NlnrPriceDyn.Logic.Common.Models;
using NlnrPriceDyn.Logic.Common.Services;
using NlnrPriceDyn.Web.Helpers;
using System.Threading.Tasks;

namespace NlnrPriceDyn.Web.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    [ApiController]
    public class AccountsController : Controller {


        private readonly IUserService _service;

        public AccountsController(IUserService service)
        {
            _service = service;
        }

        // POST api/accounts/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.RegisterUserAsync(request);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(Errors.AddErrorsToModelState(result, ModelState));

        }

        // GET api/accounts/confirmemail
        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmUserEmail([FromQuery]string id, [FromQuery]string code)
        {
            code = code.Replace(" ", "+");
            var result = await _service.ConfirmUserEmailAsync(id, code);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(Errors.AddErrorsToModelState(result, ModelState));
        }

        // GET api/accounts/resendtoken
        [HttpGet("resendtoken")]
        public async Task<IActionResult> ResendEmailValidationToken([FromQuery] string id)
        {
            var result = await _service.SendEmailConfirmationTokenAsync(id);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest(Errors.AddErrorToModelState("tokenresend_error", Resources.ErrorEmailConfirmationTokenResendError, ModelState));
            }

        }
    }
}