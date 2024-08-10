using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.DTO
{
    /// <summary>
    /// LocalizationMutatedDetailsDto
    /// </summary>
    public class LocalizationMutatedDetailsDto
    {
        /// <summary>
        /// Translation key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// OriginalTranslationEnglish by developer 
        /// </summary>
        public string OriginalTranslationEnglish { get; set; }
        /// <summary>
        /// OriginalTranslationArabic by developer 
        /// </summary>
        public string OriginalTranslationArabic { get; set; }
        /// <summary>
        /// TranslationEnglish 
        /// </summary>
        public string TranslationEnglish { get; set; }
        /// <summary>
        /// TranslationArabic 
        /// </summary>
        public string TranslationArabic { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Notes
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Number of times this translation is used in module
        /// </summary>
        public int UsageCount { get; set; }
        /// <summary>
        /// compnents paths used in 
        /// </summary>
        public string UsagePaths { get; set; }
        /// <summary>
        /// Is Translation Used For Frontend Else For Backend
        /// </summary>
        public bool IsFrontendTranslation { get; set; }
        /// <summary>
        /// ResourceName
        /// </summary>
        public string ResourceName { get; set; }
        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
