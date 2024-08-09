using ApiLocalizationProvider.Constants;
using ApiLocalizationProvider.DTO;
using ApiLocalizationProvider.Entities;
using ApiLocalizationProvider.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.BL
{
    /// <summary>
    /// LocalizationProviderService
    /// </summary>
    public class LocalizationProviderService : ILocalizationProviderService
    {
        #region PROPS
        private readonly ILocaliztionExtensionContext _dbContext;

        #endregion

        #region CTOR
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="dbContext"></param>
        public LocalizationProviderService(ILocaliztionExtensionContext dbContext)
        {
            _dbContext = dbContext;
        }


        #endregion

        #region Behaviors
        /// <summary>
        /// GetLocalizationModuleForFrontend
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Dictionary<string, string>> GetLocalizationModuleForFrontend( string language)
        {
            return await GetLocalizationModule(resourceName:null, language, isFrontend: true);
        }
        /// <summary>
        /// GetLocalizationModuleForBackEnd
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<Dictionary<string, string>> GetLocalizationModuleForBackEnd(string resourceName, string language)
        {
            return await GetLocalizationModule(resourceName, language, isFrontend: false);
        }
        #endregion

        #region Helpers
        private async Task<Dictionary<string, string>> GetLocalizationModule(string resourceName, string language, bool isFrontend)
        {
            string moduleWithPostfix = string.Empty;

            if (!isFrontend)
                moduleWithPostfix = resourceName + ".";

            var translations = await _dbContext.LocalizationDetails
                .Where(l => !l.IsDeleted && l.IsFrontendTranslation == isFrontend
                && (isFrontend || (!isFrontend && l.Key.StartsWith(moduleWithPostfix))))
                .ToListAsync();

            Dictionary<string, string> translationsKeyValue;

            switch (language)
            {
                case Languages.Arabic:
                    translationsKeyValue = GetTranslationsKeyValuePair(translations.Select(x => new LocalizationProviderDataDto
                    {
                        Key = isFrontend ? x.Key : x.Key.Substring(moduleWithPostfix.Length),
                        Value = FillArabicTranslation(x)
                    }).ToList());
                    break;

                case Languages.English:
                    translationsKeyValue = GetTranslationsKeyValuePair(translations.Select(x => new LocalizationProviderDataDto
                    {
                        Key = isFrontend ? x.Key : x.Key.Substring(moduleWithPostfix.Length),
                        Value = FillEnglishTranslation(x)
                    }).ToList());
                    break;
                default:
                    translationsKeyValue = new Dictionary<string, string>();
                    break;
            }

            return translationsKeyValue;
        }

        private string FillArabicTranslation(LocalizationDetails localizationDetails)
        {
            if (!string.IsNullOrEmpty(localizationDetails.TranslationArabic))
            {
                return localizationDetails.TranslationArabic;
            }
            else
            {
                if (!string.IsNullOrEmpty(localizationDetails.OriginalTranslationArabic))
                {
                    return localizationDetails.OriginalTranslationArabic;
                }
                else
                {
                    return FillEnglishTranslation(localizationDetails);
                }
            }
        }
        private string FillEnglishTranslation(LocalizationDetails localizationDetails)
        {
            return !string.IsNullOrEmpty(localizationDetails.TranslationEnglish) ? localizationDetails.TranslationEnglish : localizationDetails.OriginalTranslationEnglish;
        }

        private Dictionary<string, string> GetTranslationsKeyValuePair(List<LocalizationProviderDataDto> translations)
        {
            var result = new Dictionary<string, string>();

            foreach (var translation in translations)
            {
                result[translation.Key] = translation.Value;
            }
            return result;
        }
        #endregion

    }
}
