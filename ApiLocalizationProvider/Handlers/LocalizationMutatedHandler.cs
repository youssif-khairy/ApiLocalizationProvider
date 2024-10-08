﻿using ApiLocalizationProvider.Constants;
using ApiLocalizationProvider.DTO;
using ApiLocalizationProvider.Entities;
using ApiLocalizationProvider.Infrastructure;
using ApiStringLocalizer;
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
        private readonly IDbProvider _dbProvider;
        private readonly IMemoryCache _memoryCache;

        #endregion
        #region CTOR
        public LocalizationMutatedHandler(ILogger<LocalizationMutatedHandler> logger, IDbProvider dbProvider,IMemoryCache memoryCache)
        {
            _logger = logger;
            _dbProvider = dbProvider;
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
                var localization = await _dbProvider.GetLocalizationWithKeyAndTypeAsync(mutatedlocalization.Key, mutatedlocalization.IsFrontendTranslation); 

                if (localization != null && localization.Id != 0)
                {
                    if (!string.IsNullOrEmpty(localization.ResourceName))
                    {
                        resourcesChanged[localization.ResourceName] = GetResourceChangedCultures(localization, mutatedlocalization,
                            resourcesChanged.ContainsKey(localization.ResourceName) ? resourcesChanged[localization.ResourceName] : new List<string>());

                    }
                    localization.UpdateLocalization(mutatedlocalization);
                    await _dbProvider.UpdateLocalizationDetailsAsync(localization);
                }
                else
                {
                    if (!string.IsNullOrEmpty(mutatedlocalization.ResourceName))
                    {
                        resourcesChanged[mutatedlocalization.ResourceName] = new List<string> { Languages.Arabic, Languages.English };
                    }
                    await _dbProvider.InsertLocalizationDetailsAsync(new LocalizationDetails(mutatedlocalization));
                }
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
                    _memoryCache.Remove( new ApiStringLocalizerCacheKey
                    {
                        Culture = culture,
                        ResourceName = resource.Key
                    });
                }
            }
        }

        private List<string> GetResourceChangedCultures(LocalizationDetails localization, LocalizationMutatedDetailsDto mutatedlocalization, List<string> addedCultures)
        {
            var resultCultures = addedCultures ?? new List<string>();

            if (localization.TranslationArabic != mutatedlocalization.TranslationArabic && !resultCultures.Contains(Languages.Arabic))
                resultCultures.Add(Languages.Arabic);

            if (localization.TranslationEnglish != mutatedlocalization.TranslationEnglish && !resultCultures.Contains(Languages.English))
                resultCultures.Add(Languages.English);

            return resultCultures;
        }

        #endregion
    }
}
