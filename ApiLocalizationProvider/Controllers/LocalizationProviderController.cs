

using ApiLocalizationProvider.AppSettings;
using ApiLocalizationProvider.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Controllers
{
    /// <summary>
    /// LocalizationProviderController
    /// </summary>
    [AllowAnonymous]
    [Route("provider")]
    [ApiController]
    public class LocalizationProviderController : ControllerBase
    {
        #region PROPS
        private readonly ILocalizationProviderService _localizationProviderService;
        private readonly ProviderOptions _providerOptions;

        #endregion

        #region CTOR
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localizationProviderService"></param>
        /// <param name="options"></param>
        public LocalizationProviderController(ILocalizationProviderService localizationProviderService,IOptions<ApiLocalizationProviderOptions> options)
        {
            _localizationProviderService = localizationProviderService;
            _providerOptions = options.Value.ProviderOptions;
        }
        #endregion

        #region ACTIONS

        
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("frontend/{language}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, string>>> GetLocalizationModuleForFrontend(string language)
        {

            var response = await _localizationProviderService.GetLocalizationModuleForFrontend(language);

            if (_providerOptions.FrontendMaxAge.HasValue)
            {
                var cacheControl = new CacheControlHeaderValue
                {
                    MaxAge = _providerOptions.FrontendMaxAge
                };

                Response.Headers[HeaderNames.CacheControl] = cacheControl.ToString();

            }

            return response;
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("backend/{resourceName}/{language}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, string>>> GetLocalizationModuleForBackEnd(string resourceName , string language)
        {
            var response = await _localizationProviderService.GetLocalizationModuleForBackEnd(resourceName,language);

            if (_providerOptions.BackendMaxAge.HasValue)
            {

                var cacheControl = new CacheControlHeaderValue
                {
                    MaxAge = _providerOptions.BackendMaxAge
                };

                Response.Headers[HeaderNames.CacheControl] = cacheControl.ToString();
            }

            return response;
        }
      


        #endregion

    }
}
