using ApiLocalizationProvider.Constants;
using ApiLocalizationProvider.DTO;
using ApiLocalizationProvider.Entities;
using ApiLocalizationProvider.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Handlers
{
    public class LocalizationMutatedHandler : IMessageHandler<LocalizationMutatedDto>
    {
        #region PROPS
        private readonly ILogger<LocalizationMutatedHandler> _logger;
        private readonly ILocaliztionExtensionContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        #endregion
        #region CTOR
        public LocalizationMutatedHandler(ILogger<LocalizationMutatedHandler> logger, ILocaliztionExtensionContext localiztionExtensionContext,IMemoryCache memoryCache)
        {
            _logger = logger;
            _dbContext = localiztionExtensionContext;
            _memoryCache = memoryCache;
        }
        #endregion
        #region Actions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task HandleAsync(LocalizationMutatedDto value)
        {
            var resourcesChanged = new Dictionary<string, List<string>>();
            foreach (var mutatedlocalization in value.Localizations)
            {
                var localization = await _dbContext.LocalizationDetails.Where(l => l.Key == mutatedlocalization.Key && l.IsFrontendTranslation == mutatedlocalization.IsFrontendTranslation).FirstOrDefaultAsync();

                if (localization != null)
                {
                    if (!string.IsNullOrEmpty(localization.ResourceName))
                    {
                        resourcesChanged[localization.ResourceName] = GetResourceChangedCultures(localization, mutatedlocalization, resourcesChanged[localization.ResourceName]);

                    }
                    localization.UpdateLocalization(mutatedlocalization);
                }
                else
                {
                    if (!string.IsNullOrEmpty(mutatedlocalization.ResourceName))
                    {
                        resourcesChanged[mutatedlocalization.ResourceName] = new List<string> { Languages.Arabic, Languages.English };
                    }
                    _dbContext.LocalizationDetails.Add(new LocalizationDetails(mutatedlocalization));
                }
            }

            var context = _dbContext as DbContext;
            if (context != null)
            {
                context.SaveChanges();
            }

            ClearMemoryCacheForChangedResources(resourcesChanged);
        }



        public void HandleException(Exception ex, string topic, LocalizationMutatedDto value) => _logger.LogError(ex, $"{ex.Message} - {topic} - {System.Text.Json.JsonSerializer.Serialize(value)}");
        #endregion

        #region Helpers
        private void ClearMemoryCacheForChangedResources(Dictionary<string, List<string>> resourcesChanged)
        {
            foreach (var resource in resourcesChanged)
            {
                foreach (var culture in resource.Value)
                {
                    _memoryCache.Remove(new MemoryCacheKey
                    {
                        Culture = culture,
                        ResourceName = resource.Key
                    });
                }
            }
        }

        private List<string> GetResourceChangedCultures(LocalizationDetails localization, LocalizationMutatedDetailsDto mutatedlocalization, List<string> addedCultures)
        {
            if (localization.TranslationArabic != mutatedlocalization.TranslationArabic && !addedCultures.Contains(Languages.Arabic))
                addedCultures.Add(Languages.Arabic);
            else if (localization.TranslationEnglish != mutatedlocalization.TranslationEnglish && !addedCultures.Contains(Languages.English))
                addedCultures.Add(Languages.English);

            return addedCultures;
        }

        #endregion
    }
}
