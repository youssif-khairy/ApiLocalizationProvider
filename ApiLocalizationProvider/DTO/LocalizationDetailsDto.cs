using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.DTO
{
    /// <summary>
    /// LocalizationDetailsDTO
    /// </summary>
    public class LocalizationDetailsDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }
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
        /// UsageCount 
        /// </summary>
        public int UsageCount { get; set; }
        /// <summary>
        /// UsagePaths
        /// </summary>
        public List<string> UsagePaths { get; set; }
        /// <summary>
        /// IsFrontendTranslation 
        /// </summary>
        public bool IsFrontendTranslation { get; set; }
        /// <summary>
        /// ModuleId 
        /// </summary>
        public int ModuleId { get; set; }
        /// <summary>
        /// ResourceName 
        /// </summary>
        public string ResourceName { get; set; }
        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }  
    }
}
