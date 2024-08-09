
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.BL
{
    /// <summary>
    /// ILocalizationProviderService
    /// </summary>
    public interface ILocalizationProviderService
    {
        /// <summary>
        /// GetLocalizationModuleForBackEnd
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetLocalizationModuleForBackEnd(string resourceName, string language);

        /// <summary>
        /// GetLocalizationModuleForFrontend
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetLocalizationModuleForFrontend(string language);
    }
}
