

using ApiLocalizationProvider.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Controllers
{
    /// <summary>
    /// LocalizationProviderController
    /// </summary>
    
    public class LocalizationProviderController : ControllerBase
    {
        #region PROPS
        private readonly ILocalizationProviderService _localizationProviderService;
        private readonly CacheOptions _cacheOptions;

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
            _cacheOptions = options.Value.CacheOptions;
        }
        #endregion

        #region ACTIONS


        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, string>>> GetLocalizationModuleForFrontend(string language)
        {

            var response = await _localizationProviderService.GetLocalizationModuleForFrontend(language);

            if (_cacheOptions.FrontendMaxAge.HasValue)
            {
                var cacheControl = new CacheControlHeaderValue
                {
                    MaxAge = _cacheOptions.FrontendMaxAge
                };

                Response.Headers[HeaderNames.CacheControl] = cacheControl.ToString();

            }

            return response;
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, string>>> GetLocalizationModuleForBackEnd(string resourceName , string language)
        {
            var response = await _localizationProviderService.GetLocalizationModuleForBackEnd(resourceName,language);

            if (_cacheOptions.BackendMaxAge.HasValue)
            {

                var cacheControl = new CacheControlHeaderValue
                {
                    MaxAge = _cacheOptions.BackendMaxAge
                };

                Response.Headers[HeaderNames.CacheControl] = cacheControl.ToString();
            }

            return response;
        }
      


        #endregion

    }
}
