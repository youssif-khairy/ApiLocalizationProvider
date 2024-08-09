

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
        private readonly CacheSettings _chacheSettings;

        #endregion

        #region CTOR
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="correlationContextAccessor"></param>
        /// <param name="localizationProviderService"></param>
        /// <param name="chacheSettings"></param>
        public LocalizationProviderController(ILocalizationProviderService localizationProviderService,IOptions<CacheSettings> chacheSettings)
        {
            _localizationProviderService = localizationProviderService;
            _chacheSettings = chacheSettings.Value;
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

            if (_chacheSettings.FrontendMaxAge.HasValue)
            {
                var cacheControl = new CacheControlHeaderValue
                {
                    MaxAge = _chacheSettings.FrontendMaxAge
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

            if (_chacheSettings.BackendMaxAge.HasValue)
            {

                var cacheControl = new CacheControlHeaderValue
                {
                    MaxAge = _chacheSettings.BackendMaxAge
                };

                Response.Headers[HeaderNames.CacheControl] = cacheControl.ToString();
            }

            return response;
        }
      


        #endregion

    }
}
