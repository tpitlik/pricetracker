using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NlnrPriceDyn.Logic.Common;

namespace NlnrPriceDyn.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        // GET api/status
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var jsonResult = new {status = "Ok", lastupdate = TimeHelper.LastUpdateSecondsCount};
            return Ok(jsonResult);
        }
    }
}
