using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace ApiA.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDownstreamWebApi _downstreamWebApi ;

        // The Web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDownstreamWebApi downstreamWebApi)
        {
            _logger = logger;
            _downstreamWebApi  = downstreamWebApi;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            var value = await _downstreamWebApi.CallWebApiForUserAsync<object, List<WeatherForecast>>("APIB", null, options => { 
                options.RelativePath = "weatherforecast/";
                options.HttpMethod = HttpMethod.Get;
            });

            return value;
        }
    }
}
